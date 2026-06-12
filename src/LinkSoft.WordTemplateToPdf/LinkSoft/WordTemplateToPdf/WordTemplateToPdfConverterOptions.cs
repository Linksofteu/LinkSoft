namespace LinkSoft.WordTemplateToPdf;

public sealed class WordTemplateToPdfConverterOptions
{
    public string? TemporaryFolderPath { get; set; }

    public string? LibreOfficePath { get; set; }

    public TimeSpan LibreOfficeTimeout { get; set; } = TimeSpan.FromMinutes(2);
}
