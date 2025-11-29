using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;

namespace SchemaRecognizer.Core.Pdf;

public class GeometryExtractor : IGeometryExtractor
{
    public List<PdfSubpath> Extract(FileInfo fileInfo)
    {
        using var doc = PdfDocument.Open(fileInfo.FullName);
        var page = doc.GetPages().Single();
        var l = new List<PdfSubpath>();

        foreach (var path in page.Paths)
        {
            foreach (var subPath in path)
            {
                if (subPath.IsClosed())
                {
                    l.Add(subPath);
                }
            }
        }
        
        return l;
    }
}