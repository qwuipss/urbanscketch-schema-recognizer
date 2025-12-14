using SchemaRecognizer.Core.Figures;
using SchemaRecognizer.Core.Pdf;

namespace SchemaRecognizer.Core.Geo;

public interface IGeoJsonSerializer
{
    void Serialize(IEnumerable<Figure> figures, PdfFileInfo pdfFileInfo);
}