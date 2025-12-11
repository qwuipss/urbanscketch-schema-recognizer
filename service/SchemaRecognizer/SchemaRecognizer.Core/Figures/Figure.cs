using iText.Kernel.Pdf.Canvas;

namespace SchemaRecognizer.Core.Figures;

public abstract class Figure
{
    public abstract void Draw(PdfCanvas canvas);
}