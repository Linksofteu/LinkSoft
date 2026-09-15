namespace LinkSoft.WordTemplateToPdf;

public sealed class WordTemplateToPdfConverter : IWordTemplateToPdfConverter
{
    private const string DefaultLibreOfficeExecutable = "libreoffice";
    private const string DocxExtension = ".docx";
    private const string DocExtension = ".doc";
    private const string JsonExtension = ".json";
    private readonly WordTemplateToPdfConverterOptions _options;

    public WordTemplateToPdfConverter(WordTemplateToPdfConverterOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public Task<WordTemplateToPdfConversionResult> ConvertAsync(
        string docxFileName,
        string jsonFileName,
        CancellationToken cancellationToken = default)
    {
        return ConvertAsync(
            new WordTemplateToPdfConversionRequest(docxFileName, jsonFileName),
            cancellationToken);
    }

    public async Task<WordTemplateToPdfConversionResult> ConvertAsync(
        WordTemplateToPdfConversionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var temporaryRoot = GetConfiguredTemporaryRoot();
        var templatePath = ValidateTemplatePath(ResolveRunFilePath(request.DocxFileName, temporaryRoot));
        var jsonPath = ValidateJsonPath(ResolveRunFilePath(request.JsonFileName, temporaryRoot));
        var templateData = await TemplateDataReader.ReadAsync(jsonPath, cancellationToken).ConfigureAwait(false);

        var workingDirectory = temporaryRoot;
        var filledDocxPath = Path.Combine(workingDirectory, $"NEW_{Path.GetFileName(templatePath)}");

        MiniWordTemplateRenderer.SaveFilledDocument(templatePath, filledDocxPath, templateData, cancellationToken);

        var pdfConverter = new LibreOfficePdfConverter(GetLibreOfficeExecutable(), _options.LibreOfficeTimeout);
        var pdfPath = await pdfConverter.ConvertAsync(filledDocxPath, workingDirectory, cancellationToken).ConfigureAwait(false);

        return new WordTemplateToPdfConversionResult(pdfPath, filledDocxPath, workingDirectory);
    }

    private string GetConfiguredTemporaryRoot()
    {
        if (string.IsNullOrWhiteSpace(_options.TemporaryFolderPath))
        {
            throw new InvalidOperationException("TemporaryFolderPath must be configured before converting a document.");
        }

        if (_options.LibreOfficeTimeout <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("LibreOfficeTimeout must be greater than zero.");
        }

        var temporaryRoot = Path.GetFullPath(_options.TemporaryFolderPath.Trim());
        Directory.CreateDirectory(temporaryRoot);
        return temporaryRoot;
    }

    private string GetLibreOfficeExecutable()
    {
        return string.IsNullOrWhiteSpace(_options.LibreOfficePath)
            ? DefaultLibreOfficeExecutable
            : _options.LibreOfficePath.Trim();
    }

    private static string ResolveRunFilePath(string fileNameOrPath, string temporaryRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileNameOrPath);
        var trimmedPath = fileNameOrPath.Trim();

        if (Path.IsPathRooted(trimmedPath))
        {
            return trimmedPath;
        }

        var temporaryFolderPath = Path.GetFullPath(Path.Combine(temporaryRoot, trimmedPath));
        if (File.Exists(temporaryFolderPath))
        {
            return temporaryFolderPath;
        }

        var currentDirectoryPath = Path.GetFullPath(trimmedPath);
        return File.Exists(currentDirectoryPath)
            ? currentDirectoryPath
            : temporaryFolderPath;
    }

    private static string ValidateTemplatePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var fullPath = Path.GetFullPath(path.Trim());

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("DOCX template file was not found.", fullPath);
        }

        var extension = Path.GetExtension(fullPath);
        if (extension.Equals(DocExtension, StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Unsupported format, please convert to .docx first.");
        }

        if (!extension.Equals(DocxExtension, StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Only .docx templates are supported.");
        }

        return fullPath;
    }

    private static string ValidateJsonPath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var fullPath = Path.GetFullPath(path.Trim());

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("JSON template data file was not found.", fullPath);
        }

        if (!Path.GetExtension(fullPath).Equals(JsonExtension, StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Only .json template data files are supported.");
        }

        return fullPath;
    }

}
