using Microsoft.Extensions.DependencyInjection;
using SchemaRecognizer.Core.Pdf;
using SchemaRecognizer.Core.Pdf.Drawing;
using SchemaRecognizer.Core.Pdf.Filtering;
using SchemaRecognizer.Core.Pdf.Utilities;

namespace SchemaRecognizer.Worker.Setup;

internal static class ServicesSetup
{
    public static IServiceCollection SetupAppServices(this IServiceCollection services)
    {
        return services
               .AddSingleton<IPdfTypeDetector, PdfTypeDetector>()
               .AddSingleton<IPdfValidator, PdfValidator>()
               .AddSingleton<IPdfPathFilter, PdfPathFilter>()
               .AddSingleton<IPdfFiguresExtractor, PdfFiguresExtractor>()
               .AddSingleton<IPdfDrawer, PdfDrawer>()
               .AddSingleton<IExecutor, Executor>();
    }
}