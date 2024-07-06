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
        // If this causes issues running locally, make sure there are no "not logged in" warnings
        
        return configuration;
    }
}
