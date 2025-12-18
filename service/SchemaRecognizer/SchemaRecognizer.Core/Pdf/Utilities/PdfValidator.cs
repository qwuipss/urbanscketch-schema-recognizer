using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using SchemaRecognizer.Core.Helpers;
using SkiaSharp;
using UglyToad.PdfPig;
using PdfDocument = UglyToad.PdfPig.PdfDocument;

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
            throw new NotSupportedException($"File {fileInfo.FullName} is not pdf");
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

    public void ValidatePdfRasterization(PdfFileInfo fileInfo)
    {
        var rasterPdfFilePath = _options.Value.RasterPdfFilePath;
        using var bitmap = SKBitmap.Decode(rasterPdfFilePath);

        if (bitmap is null)
        {
            throw new ArgumentException($"Unable to load image '{rasterPdfFilePath}'", nameof(fileInfo));
        }

        if (MathHelper.AreEqual(fileInfo.Width, bitmap.Width) && MathHelper.AreEqual(fileInfo.Height, bitmap.Height))
        {
            return;
        }

        throw new InvalidOperationException(
            $"Vector and raster pdf dimensions mismatch. "
            + $"Vector: {fileInfo.Width}x{fileInfo.Height}pt. "
            + $"Raster: {bitmap.Width}x{bitmap.Height}px"
        );
    }

    private static (double Width, double Height) GetPageSize(PdfDocument document)
    {
        var page = document.GetPages().Single();

        return (page.Width, page.Height);
    }
}