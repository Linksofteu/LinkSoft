namespace LinkSoft.WordTemplateToPdf;

public sealed record WordTemplateToPdfConversionResult(
    string PdfPath,
    string FilledDocxPath,
    string WorkingDirectory);
