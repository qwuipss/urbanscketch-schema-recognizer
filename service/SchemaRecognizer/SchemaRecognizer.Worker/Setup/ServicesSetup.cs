using Microsoft.Extensions.DependencyInjection;
using SchemaRecognizer.Core.Pdf;

namespace SchemaRecognizer.Worker.Setup;

internal static class ServicesSetup
{
    public static IServiceCollection SetupAppServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IPdfTypeDetector, PdfTypeDetector>()
            .AddSingleton<IPdfValidator, PdfValidator>()
            .AddSingleton<IExecutor, Executor>();
    }
}