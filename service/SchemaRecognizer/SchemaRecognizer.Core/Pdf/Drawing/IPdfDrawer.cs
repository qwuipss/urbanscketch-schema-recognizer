using SchemaRecognizer.Core.Figures;

namespace SchemaRecognizer.Core.Pdf.Drawing;

public interface IPdfDrawer
{
    void Draw(ICollection<Figure> figures, (double Width, double Height) pageSize);
}