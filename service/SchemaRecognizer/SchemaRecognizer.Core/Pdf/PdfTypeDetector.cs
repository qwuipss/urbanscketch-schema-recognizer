using UglyToad.PdfPig;

namespace SchemaRecognizer.Core.Pdf;

public class PdfTypeDetector : IPdfTypeDetector
{
    public PdfType Detect(FileInfo fileInfo)
    {
        using var doc = PdfDocument.Open(fileInfo.FullName);
        var page = doc.GetPages().Single();

        var isVector = page.Text.Length is not 0 || page.Paths.Count is not 0;

        return isVector ? PdfType.Vector : PdfType.Raster;
    }
}