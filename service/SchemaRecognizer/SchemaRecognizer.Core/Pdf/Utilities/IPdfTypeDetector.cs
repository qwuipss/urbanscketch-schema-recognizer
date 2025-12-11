namespace SchemaRecognizer.Core.Pdf.Utilities;

public interface IPdfTypeDetector
{
    PdfType Detect(FileInfo fileInfo);
}