using Microsoft.Extensions.DependencyInjection;
using SchemaRecognizer.Worker;
using SchemaRecognizer.Worker.Setup;

var services = new ServiceCollection();

services
    .SetupAppServices()
    .SetupAppConfiguration()
    .SetupAppLogging();

var serviceProvider = services.BuildServiceProvider();

var filePath = Path.Join(Environment.CurrentDirectory, "../../../../", "pdf/vector/v1.pdf");
var fileInfo = new FileInfo(filePath);
var worker = serviceProvider.GetRequiredService<IExecutor>();

worker.Run(fileInfo);