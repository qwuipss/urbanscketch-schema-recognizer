namespace SchemaRecognizer.Core.Pdf.Utilities;

public interface IPdfValidator
{
    PdfFileInfo Validate(FileInfo fileInfo);

    void ValidatePdfRasterization(PdfFileInfo fileInfo);
}