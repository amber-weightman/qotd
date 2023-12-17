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
                "Endpoint=https://questionoftheday.azconfig.io;Id=SL46;Secret=STQlDTKrj5gKx3/xPpCDRG4SfM9nSmEy/XJHt/cciYE=")
                    //configuration["ConnectionStrings:AppConfig"])
                    .ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(new DefaultAzureCredential());
                    });
        });

        return configuration;
    }
}
