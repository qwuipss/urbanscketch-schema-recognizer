using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using UglyToad.PdfPig;

namespace SchemaRecognizer.Core.Pdf.Utilities;

public class PdfValidator(IOptions<PdfSchemaOptions> options) : IPdfValidator
{
    private readonly IOptions<PdfSchemaOptions> _options = options;

    public PdfFileInfo Validate(FileInfo fileInfo)
    {
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"File {fileInfo.FullName} was not found");
        }

        if (!fileInfo.Extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException($"File {fileInfo.FullName} is not PDF");
        }

        using var document = PdfDocument.Open(fileInfo.FullName);

        if (document.NumberOfPages is not 1)
        {
            throw new NotSupportedException($"File {fileInfo.FullName} has more than 1 page");
        }

        var (width, height) = GetPageSize(document);
        var scale = _options.Value.Scale;

        return new PdfFileInfo(width, height, scale, fileInfo);
    }

    private static (double Width, double Height) GetPageSize(PdfDocument document)
    {
        var page = document.GetPages().Single();

        return (page.Width, page.Height);
    }
}