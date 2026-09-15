using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace LinkSoft.WordTemplateToPdf;

internal static class IndexedArrayRowExpander
{
    private static readonly Regex IndexedArrayPathPattern = new(
        @"(?<array>[A-Za-z_][A-Za-z0-9_]*(?:\.[A-Za-z_][A-Za-z0-9_]*)*)\[(?<index>\d+)\]",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static byte[] Expand(byte[] templateBytes, IReadOnlyDictionary<string, object?> templateData)
    {
        var arrayCounts = GetArrayCounts(templateData);
        if (arrayCounts.Count == 0)
        {
            return templateBytes;
        }

        using var stream = new MemoryStream();
        stream.Write(templateBytes, 0, templateBytes.Length);
        stream.Position = 0;

        using (var document = WordprocessingDocument.Open(stream, true))
        {
            ExpandTables(document.MainDocumentPart?.Document, arrayCounts);

            foreach (var header in document.MainDocumentPart?.HeaderParts.Select(part => part.Header) ?? Enumerable.Empty<Header>())
            {
                ExpandTables(header, arrayCounts);
            }

            foreach (var footer in document.MainDocumentPart?.FooterParts.Select(part => part.Footer) ?? Enumerable.Empty<Footer>())
            {
                ExpandTables(footer, arrayCounts);
            }

            document.Save();
        }

        return stream.ToArray();
    }

    private static Dictionary<string, int> GetArrayCounts(IReadOnlyDictionary<string, object?> templateData)
    {
        var arrayCounts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var (key, value) in templateData)
        {
            const string countSuffix = ".Count";
            if (!key.EndsWith(countSuffix, StringComparison.Ordinal) || value is null)
            {
                continue;
            }

            var arrayName = key[..^countSuffix.Length];
            if (arrayName.Length == 0)
            {
                continue;
            }

            if (TryConvertToNonNegativeCount(value, out var count))
            {
                arrayCounts[arrayName] = count;
            }
        }

        return arrayCounts;
    }

    private static bool TryConvertToNonNegativeCount(object value, out int count)
    {
        switch (value)
        {
            case int intValue when intValue >= 0:
                count = intValue;
                return true;
            case long longValue when longValue is >= 0 and <= int.MaxValue:
                count = (int)longValue;
                return true;
            case decimal decimalValue when decimalValue is >= 0 and <= int.MaxValue && decimal.Truncate(decimalValue) == decimalValue:
                count = (int)decimalValue;
                return true;
            default:
                count = 0;
                return false;
        }
    }

    private static void ExpandTables(OpenXmlPartRootElement? rootElement, IReadOnlyDictionary<string, int> arrayCounts)
    {
        if (rootElement is null)
        {
            return;
        }

        foreach (var table in rootElement.Descendants<Table>().ToList())
        {
            ExpandTable(table, arrayCounts);
        }
    }

    private static void ExpandTable(Table table, IReadOnlyDictionary<string, int> arrayCounts)
    {
        var rows = table.Elements<TableRow>().ToList();
        for (var rowIndex = 0; rowIndex < rows.Count;)
        {
            var rowTemplate = GetArrayRowTemplate(rows[rowIndex], arrayCounts);
            if (rowTemplate is null)
            {
                rowIndex++;
                continue;
            }

            var group = new List<ArrayRowTemplate>();
            while (rowIndex < rows.Count)
            {
                var nextTemplate = GetArrayRowTemplate(rows[rowIndex], arrayCounts);
                if (nextTemplate is null || !string.Equals(nextTemplate.ArrayName, rowTemplate.ArrayName, StringComparison.Ordinal))
                {
                    break;
                }

                group.Add(nextTemplate);
                rowIndex++;
            }

            ReplaceTemplateRows(group, arrayCounts[rowTemplate.ArrayName]);
        }
    }

    private static ArrayRowTemplate? GetArrayRowTemplate(TableRow row, IReadOnlyDictionary<string, int> arrayCounts)
    {
        var rowText = string.Concat(row.Descendants<Text>().Select(text => text.Text ?? string.Empty));
        var matches = IndexedArrayPathPattern
            .Matches(rowText)
            .Where(match => arrayCounts.ContainsKey(match.Groups["array"].Value))
            .Select(match => new
            {
                ArrayName = match.Groups["array"].Value,
                Index = int.Parse(match.Groups["index"].Value, System.Globalization.CultureInfo.InvariantCulture)
            })
            .Distinct()
            .ToList();

        if (matches.Count == 0)
        {
            return null;
        }

        var arrayName = matches[0].ArrayName;
        var index = matches[0].Index;
        if (matches.Any(match =>
                !string.Equals(match.ArrayName, arrayName, StringComparison.Ordinal) ||
                match.Index != index))
        {
            return null;
        }

        return new ArrayRowTemplate(row, arrayName, index);
    }

    private static void ReplaceTemplateRows(IReadOnlyList<ArrayRowTemplate> templateRows, int itemCount)
    {
        if (templateRows.Count == 0)
        {
            return;
        }

        var firstTemplateRow = templateRows[0].Row;
        for (var itemIndex = 0; itemIndex < itemCount; itemIndex++)
        {
            var templateRow = templateRows[itemIndex % templateRows.Count];
            var expandedRow = (TableRow)templateRow.Row.CloneNode(deep: true);
            RewriteArrayIndex(expandedRow, templateRow.ArrayName, templateRow.Index, itemIndex);
            firstTemplateRow.InsertBeforeSelf(expandedRow);
        }

        foreach (var templateRow in templateRows)
        {
            templateRow.Row.Remove();
        }
    }

    private static void RewriteArrayIndex(OpenXmlElement element, string arrayName, int oldIndex, int newIndex)
    {
        var oldArrayPath = $"{arrayName}[{oldIndex}]";
        var newArrayPath = $"{arrayName}[{newIndex}]";
        foreach (var text in element.Descendants<Text>())
        {
            text.Text = (text.Text ?? string.Empty).Replace(oldArrayPath, newArrayPath, StringComparison.Ordinal);
        }
    }

    private sealed record ArrayRowTemplate(TableRow Row, string ArrayName, int Index);
}
