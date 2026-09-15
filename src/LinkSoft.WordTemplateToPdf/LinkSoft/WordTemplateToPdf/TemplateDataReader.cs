using System.Text.Json;

namespace LinkSoft.WordTemplateToPdf;

internal static class TemplateDataReader
{
    public static async Task<IReadOnlyDictionary<string, object?>> ReadAsync(
        string jsonPath,
        CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(jsonPath);
        using var document = await JsonDocument.ParseAsync(
            stream,
            new JsonDocumentOptions { AllowTrailingCommas = true },
            cancellationToken).ConfigureAwait(false);

        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("Template data JSON must contain an object at the root.");
        }

        var data = new Dictionary<string, object?>(StringComparer.Ordinal);
        AddTemplateValues(document.RootElement, data, path: null);

        return data;
    }

    private static object? ConvertValue(JsonElement jsonValue)
    {
        return jsonValue.ValueKind switch
        {
            JsonValueKind.String => jsonValue.GetString(),
            JsonValueKind.Number when jsonValue.TryGetInt64(out var longValue) => longValue,
            JsonValueKind.Number when jsonValue.TryGetDecimal(out var decimalValue) => decimalValue,
            JsonValueKind.Number => jsonValue.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => null
        };
    }

    private static void AddTemplateValues(
        JsonElement jsonValue,
        IDictionary<string, object?> data,
        string? path)
    {
        switch (jsonValue.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in jsonValue.EnumerateObject())
                {
                    var propertyName = property.Name.Trim();
                    if (propertyName.Length == 0)
                    {
                        continue;
                    }

                    var propertyPath = string.IsNullOrWhiteSpace(path)
                        ? propertyName
                        : $"{path}.{propertyName}";

                    AddTemplateValues(property.Value, data, propertyPath);
                }

                break;

            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in jsonValue.EnumerateArray())
                {
                    AddTemplateValues(item, data, $"{path}[{index}]");
                    index++;
                }

                if (!string.IsNullOrWhiteSpace(path))
                {
                    AddTemplateValue(data, $"{path}.Count", index);
                    AddScalarArrayValue(data, path, jsonValue);
                }

                break;

            default:
                if (!string.IsNullOrWhiteSpace(path))
                {
                    AddTemplateValue(data, path, ConvertValue(jsonValue));
                }

                break;
        }
    }

    private static void AddTemplateValue(IDictionary<string, object?> data, string key, object? value)
    {
        if (!data.ContainsKey(key))
        {
            data[key] = value;
        }
    }

    private static void AddScalarArrayValue(IDictionary<string, object?> data, string key, JsonElement jsonArray)
    {
        var scalarValues = new List<string>();
        foreach (var item in jsonArray.EnumerateArray())
        {
            if (item.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
            {
                return;
            }

            var value = ConvertValue(item);
            if (value is not null)
            {
                scalarValues.Add(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty);
            }
        }

        AddTemplateValue(data, key, string.Join(", ", scalarValues));
    }
}
