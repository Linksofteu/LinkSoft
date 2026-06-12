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

        return ConvertObject(document.RootElement);
    }

    private static Dictionary<string, object?> ConvertObject(JsonElement jsonObject)
    {
        var data = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var property in jsonObject.EnumerateObject())
        {
            data[property.Name.Trim()] = ConvertValue(property.Value);
        }

        return data;
    }

    private static object? ConvertValue(JsonElement jsonValue)
    {
        return jsonValue.ValueKind switch
        {
            JsonValueKind.Object => ConvertObject(jsonValue),
            JsonValueKind.Array => ConvertArray(jsonValue),
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

    private static object ConvertArray(JsonElement jsonArray)
    {
        var values = jsonArray.EnumerateArray().Select(ConvertValue).ToList();

        if (values.All(value => value is string))
        {
            return values.Cast<string>().ToList();
        }

        if (values.All(value => value is Dictionary<string, object?>))
        {
            return values.Cast<Dictionary<string, object?>>().ToList();
        }

        return values;
    }
}
