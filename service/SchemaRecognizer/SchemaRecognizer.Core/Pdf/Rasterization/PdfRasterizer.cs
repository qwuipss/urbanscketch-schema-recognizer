using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using SchemaRecognizer.Core.Helpers;
using ImageType = IronPdf.Imaging.ImageType;

namespace SchemaRecognizer.Core.Pdf.Rasterization;

public sealed class PdfRasterizer(IOptions<PdfSchemaOptions> options) : IPdfRasterizer
{
    private readonly IOptions<PdfSchemaOptions> _options = options;

    public void Rasterize(PdfFileInfo pdfFileInfo)
    {
        var pdfDocument = PdfDocument.FromFile(pdfFileInfo.FileInfo.FullName);
        var rasterFileInfo = FilesHelper.GetFileInfoByPath(_options.Value.RasterPdfFilePath);

        pdfDocument.RasterizeToImageFiles(rasterFileInfo.FullName, ImageType.Default, Constants.PtToPxDpi);
    }
}