namespace SchemaRecognizer.Core.Pdf;

public interface IPdfTypeDetector
{
    PdfType Detect(FileInfo fileInfo);
}