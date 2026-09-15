The purpose of this package is to fill DOCX templates from JSON data and convert the generated document to PDF using LibreOffice headless mode.

The package targets the repository-wide .NET version configured in `common/common.props` (`net9.0`).

Configuration:

```csharp
using LinkSoft.WordTemplateToPdf;

var converter = new WordTemplateToPdfConverter(new WordTemplateToPdfConverterOptions
{
    // Required. Output and LibreOffice working files are created here.
    TemporaryFolderPath = "/var/tmp/linksoft-documents",

    // Optional. If omitted, `libreoffice` is expected to be available in PATH.
    LibreOfficePath = "/usr/bin/libreoffice"
});
```

Usage:

```csharp
var result = await converter.ConvertAsync(
    docxFileName: "contract.docx",
    jsonFileName: "contract-data.json");

Console.WriteLine(result.PdfPath);
```

The DOCX file name and JSON file name are supplied for every conversion run. Relative names are resolved from `TemporaryFolderPath` first and then from the current working directory; rooted paths are used as-is. The JSON root must be an object. Property names map directly to template placeholders without brackets.

Output files are written directly into `TemporaryFolderPath` as `NEW_<docx file name>` and `NEW_<docx file name without extension>.pdf`.

```json
{
  "CustomerName": "Jane Doe",
  "ContractNumber": "C-2026-0001",
  "Amount": 123.45
}
```

Templates must use `[[Name]]` placeholders. For example, the JSON property `Name` fills the Word placeholder `[[Name]]`.

This package is a part of larger set of packages for LinkSoft Technologies shared open source repository.

You can find the repository on [GitHub](https://github.com/Linksofteu/LinkSoft).
