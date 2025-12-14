using iText.Kernel.Pdf.Canvas;
using SchemaRecognizer.Core.Pdf;

namespace SchemaRecognizer.Core.Figures;

public abstract class Figure
{
    public abstract void Draw(PdfCanvas canvas);
    public abstract object ToGeoJsonFeature(PdfFileInfo pdfFileInfo);
}