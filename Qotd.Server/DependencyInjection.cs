using Azure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Qotd.Api;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static ConfigurationManager ConfigureAzure(this ConfigurationManager configuration)
    {
        configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(
                configuration["ConnectionStrings:AppConfig"])
                    .ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(new DefaultAzureCredential());
                    });
        });

        return configuration;
    }
}
