using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using SchemaRecognizer.Core.Figures;
using Path = System.IO.Path;

namespace SchemaRecognizer.Core.Pdf.Drawing;

public sealed class PdfDrawer(IOptions<PdfPathFilterOptions> options) : IPdfDrawer
{
    private readonly IOptions<PdfPathFilterOptions> _options = options;

    public void Draw(ICollection<Figure> figures, (double Width, double Height) pageSize)
    {
        var fileInfo = GetDrawFiguresFileInfo();
        var stream = fileInfo.Open(FileMode.CreateNew, FileAccess.Write);

        using var writer = new PdfWriter(stream);
        using var document = new PdfDocument(writer);

        var page = document.AddNewPage(new PageSize((float)pageSize.Width, (float)pageSize.Height));
        var canvas = new PdfCanvas(page);

        foreach (var figure in figures)
        {
            figure.Draw(canvas);
        }
    }

    private FileInfo GetDrawFiguresFileInfo()
    {
        var drawFiguresFilePath = _options.Value.DrawFiguresFilePath;

        var fileInfo = Path.IsPathRooted(drawFiguresFilePath)
            ? new FileInfo(drawFiguresFilePath)
            : new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, drawFiguresFilePath));

        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }

        return fileInfo;
    }
}