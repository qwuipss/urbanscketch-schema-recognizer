using PdfDocument = UglyToad.PdfPig.PdfDocument;

namespace SchemaRecognizer.Debug;

internal static class PdfReader
{
    public static void Read(string filePath)
    {
        using var document = PdfDocument.Open(filePath);

        foreach (var page in document.GetPages())
        {
            Console.WriteLine("Text:");
            Console.WriteLine(page.Text);
            Console.WriteLine();

            var paths = page.Paths;
            Console.WriteLine($"Path count: {paths.Count}");

            foreach (var path in paths)
            {
            }
        }
    }
}