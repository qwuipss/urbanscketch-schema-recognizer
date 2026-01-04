using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SchemaRecognizer.Core.Configuration;
using SchemaRecognizer.Core.Configuration.Configure;

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
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<IConfigureOptions<PdfPathFilterOptions>, PdfPathFilterOptionsConfigure>();
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