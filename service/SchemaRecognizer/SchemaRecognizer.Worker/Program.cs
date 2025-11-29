using Microsoft.Extensions.DependencyInjection;
using SchemaRecognizer.Worker;
using SchemaRecognizer.Worker.Setup;

var services = new ServiceCollection();

services
    .SetupAppServices()
    .SetupAppLogging();

var serviceProvider = services.BuildServiceProvider();

var filePath = Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/vector/v2.pdf");
var fileInfo = new FileInfo(filePath);
var worker = serviceProvider.GetRequiredService<IExecutor>();

worker.Run(fileInfo);