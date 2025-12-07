using SchemaRecognizer.Core.Extensions;
using SchemaRecognizer.Core.Geometry;
using UglyToad.PdfPig;

namespace SchemaRecognizer.Core.Pdf;

public class GeometryExtractor : IGeometryExtractor
{
    public ICollection<Figure> Extract(FileInfo fileInfo)
    {
        using var doc = PdfDocument.Open(fileInfo.FullName);
        var page = doc.GetPages().Single();
        var figures = new List<Figure>();

        foreach (var path in page.Paths)
        {
            foreach (var subPath in path)
            {
                if (subPath.IsClosed() && subPath.HasClose() && !subPath.HasBezierCurve())
                {
                    var polygon = new Polygon(subPath);
                    figures.Add(polygon);
                }
            }
        }

        return figures;
    }
}