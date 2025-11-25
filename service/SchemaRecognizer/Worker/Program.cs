using Microsoft.Extensions.DependencyInjection;
using Worker.Pdf;
using Worker.Setup;

var services = new ServiceCollection();

services.SetupAppLogging();

var path = Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/2.pdf");
var x = PdfTypeDetector.Detect(path);
