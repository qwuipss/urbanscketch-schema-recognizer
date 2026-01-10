namespace SchemaRecognizer.Core.Pdf.Rasterization;

public interface IPdfRasterizer
{
    void Rasterize(PdfFileInfo pdfFileInfo);
}