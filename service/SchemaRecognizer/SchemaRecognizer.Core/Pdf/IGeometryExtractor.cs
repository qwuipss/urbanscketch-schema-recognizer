using SchemaRecognizer.Core.Geometry;

namespace SchemaRecognizer.Core.Pdf;

public interface IGeometryExtractor
{
    ICollection<Figure> Extract(FileInfo fileInfo);
}