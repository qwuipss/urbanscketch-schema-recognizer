using iText.Kernel.Pdf.Canvas;

namespace SchemaRecognizer.Core.Geometry;

public abstract class Figure
{
    public abstract void Draw(PdfCanvas canvas);
}