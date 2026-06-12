namespace LinkSoft.WordTemplateToPdf;

public interface IWordTemplateToPdfConverter
{
    Task<WordTemplateToPdfConversionResult> ConvertAsync(
        WordTemplateToPdfConversionRequest request,
        CancellationToken cancellationToken = default);

    Task<WordTemplateToPdfConversionResult> ConvertAsync(
        string docxFileName,
        string jsonFileName,
        CancellationToken cancellationToken = default);
}
