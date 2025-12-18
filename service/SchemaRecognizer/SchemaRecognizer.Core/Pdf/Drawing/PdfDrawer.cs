using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using SchemaRecognizer.Core.Figures;
using SchemaRecognizer.Core.Helpers;
using Path = System.IO.Path;
using PdfDocument = iText.Kernel.Pdf.PdfDocument;

namespace SchemaRecognizer.Core.Pdf.Drawing;

public sealed class PdfDrawer(IOptions<PdfPathFilterOptions> filterOptions, IOptions<PdfSchemaOptions> schemaOptions) : IPdfDrawer
{
    private readonly IOptions<PdfPathFilterOptions> _filterOptions = filterOptions;
    private readonly IOptions<PdfSchemaOptions> _schemaOptions = schemaOptions;

    public void Draw(ICollection<Figure> figures, PdfFileInfo pdfFileInfo)
    {
        var fileInfo = FilesHelper.GetFileInfoByPath(_schemaOptions.Value.DrawFiguresFilePath);
        var stream = fileInfo.Open(FileMode.CreateNew, FileAccess.Write);

        using var writer = new PdfWriter(stream);
        using var document = new PdfDocument(writer);

        var page = document.AddNewPage(new PageSize((float)pdfFileInfo.Width, (float)pdfFileInfo.Height));
        var canvas = new PdfCanvas(page);

        foreach (var figure in figures)
        {
            figure.Draw(canvas);
        }
    }
}