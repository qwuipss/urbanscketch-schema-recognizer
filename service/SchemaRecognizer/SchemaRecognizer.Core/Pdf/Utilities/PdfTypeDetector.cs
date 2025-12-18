using PdfDocument = UglyToad.PdfPig.PdfDocument;

namespace SchemaRecognizer.Core.Pdf.Utilities;

public class PdfTypeDetector : IPdfTypeDetector
{
    public PdfType Detect(FileInfo fileInfo)
    {
        using var doc = PdfDocument.Open(fileInfo.FullName);
        var page = doc.GetPages().Single();

        var isVector = page.Paths.Count is not 0;

        return isVector ? PdfType.Vector : PdfType.Raster;
    }
}