using UglyToad.PdfPig;

var path = Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/vector/v1.pdf");

ReadPdfWithPdfPig(path);

static void ReadPdfWithPdfPig(string filePath)
{
    Console.WriteLine("\n=== ЧТЕНИЕ PDF С ПОМОЩЬЮ PdfPig ===\n");

    try
    {
        using var document = PdfDocument.Open(filePath);

        Console.WriteLine($"Количество страниц: {document.NumberOfPages}");
        Console.WriteLine($"Версия PDF: {document.Version}");
        Console.WriteLine($"Зашифрован: {document.IsEncrypted}");
        Console.WriteLine();

        foreach (var page in document.GetPages())
        {
            Console.WriteLine($"=== Страница {page.Number} ===");

            // Извлекаем текст
            Console.WriteLine("ТЕКСТ:");
            Console.WriteLine(page.Text);
            Console.WriteLine();

            // Информация о векторной графике
            Console.WriteLine("ВЕКТОРНАЯ ГРАФИКА:");
            var paths = page.Paths;
            Console.WriteLine($"Количество векторных путей: {paths.Count()}");

            foreach (var path in paths) // Покажем первые 5 путей
            {
                var t = path.IsFilled;
                if (t)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при чтении PDF: {ex.Message}");
    }
}