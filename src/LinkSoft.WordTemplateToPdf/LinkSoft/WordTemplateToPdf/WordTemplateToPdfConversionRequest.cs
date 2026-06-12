namespace LinkSoft.WordTemplateToPdf;

public sealed record WordTemplateToPdfConversionRequest(
    string DocxFileName,
    string JsonFileName);
