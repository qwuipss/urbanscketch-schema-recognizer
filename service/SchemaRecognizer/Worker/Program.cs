using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.Extensions.DependencyInjection;
using Worker.Pdf;
using Worker.Setup;

var services = new ServiceCollection();

services.SetupAppLogging();

var path = Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/1.pdf");
var x = PdfTypeDetector.Detect(path);

var d = ExtractVectorGeometry(path);

foreach (var shp in d)
{
    Console.WriteLine($"{shp.Operation} | Points: {shp.Points.Count} | Fill: {shp.ColorFill} | Stroke: {shp.ColorStroke}");
}
return;
static List<VectorShape> ExtractVectorGeometry(string path)
{
    using var reader = new PdfReader(path);
    using var pdf = new PdfDocument(reader);

    var collector = new VectorShapeCollector();

    for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
    {
        var page = pdf.GetPage(i);
        var processor = new PdfCanvasProcessor(collector);
        processor.ProcessPageContent(page);
    }

    return collector.Shapes;
}