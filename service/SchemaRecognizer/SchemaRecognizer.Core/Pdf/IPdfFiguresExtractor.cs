using SchemaRecognizer.Core.Figures;

namespace SchemaRecognizer.Core.Pdf;

public interface IPdfFiguresExtractor
{
    ICollection<Figure> Extract(FileInfo fileInfo);
}