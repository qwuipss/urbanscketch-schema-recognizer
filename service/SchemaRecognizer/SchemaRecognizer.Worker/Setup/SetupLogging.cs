using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SchemaRecognizer.Worker.Setup;

public static class SetupLogging
{
    public static IServiceCollection SetupAppLogging(this IServiceCollection services)
    {
        const string logMessageTemplate =
            "[{Timestamp:HH:mm:ss.fff}] {Level:u3} [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        var loggerConfig = new LoggerConfiguration()
                           .MinimumLevel.Information()
                           .WriteTo.Console(outputTemplate: logMessageTemplate);

        Log.Logger = loggerConfig.CreateLogger();

        services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog();
            }
        );

        return services;
    }
}