using MiniSoftware;

namespace LinkSoft.WordTemplateToPdf;

internal static class MiniWordTemplateRenderer
{
    public static void SaveFilledDocument(
        string templatePath,
        string outputDocxPath,
        IReadOnlyDictionary<string, object?> templateData,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedTemplate = SquareBracketTagNormalizer.Normalize(templatePath);
        MiniWord.SaveAsByTemplate(outputDocxPath, normalizedTemplate, templateData);
    }
}
