using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchemaRecognizer.Core.Configuration;

namespace SchemaRecognizer.Worker.Setup;

internal static class ConfigurationSetup
{
    public static IServiceCollection SetupAppConfiguration(this IServiceCollection services)
    {
        var configurationBuilder = new ConfigurationBuilder();

        var configuration = configurationBuilder
                            .SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("configuration.json")
                            .Build();

        AddAppOptions(services, configuration);

        return services
            .AddSingleton<IConfiguration>(configuration);
    }

    private static void AddAppOptions(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptionsWithValidateOnStart<PdfPathFilterOptions>()
            .ValidateDataAnnotations()
            .Bind(configuration);
        
        services
            .AddOptionsWithValidateOnStart<PdfSchemaOptions>()
            .ValidateDataAnnotations()
            .Bind(configuration);
    }
}