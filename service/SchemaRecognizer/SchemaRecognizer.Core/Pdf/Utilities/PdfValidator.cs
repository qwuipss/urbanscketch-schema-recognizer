using UglyToad.PdfPig;

namespace SchemaRecognizer.Core.Pdf.Utilities;

public class PdfValidator : IPdfValidator
{
    public void Validate(FileInfo fileInfo)
    {
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"File {fileInfo.FullName} was not found");
        }

        if (!fileInfo.Extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException($"File {fileInfo.FullName} is not PDF");
        }
        
        using var doc = PdfDocument.Open(fileInfo.FullName);

        if (doc.NumberOfPages is not 1)
        {
            throw  new NotSupportedException($"File {fileInfo.FullName} has more than 1 page");
        }
    }
}