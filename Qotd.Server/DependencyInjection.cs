using Azure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Qotd.Api;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static ConfigurationManager ConfigureAzure(this ConfigurationManager configuration)
    {

        //builder.Logging.AddConsole();

        // AADSTS500200: User account 'amberweightman@hotmail.com' is a personal Microsoft account. Personal Microsoft accounts are not
        // supported for this application unless explicitly invited to an organization. Try signing out and signing back in with an organizational account.

        //builder.AddConsole(LogLevel.Debug)
        //.AddDebug();

        //var logFactory = new LoggerFactory()
        //.AddConsole(LogLevel.Debug)
        //.AddDebug();

        //var logger = logFactory.CreateLogger<Type>();

        //logger.LogInformation("this is debug log");

        var options = new DefaultAzureCredentialOptions()
        {
            ExcludeAzurePowerShellCredential = true,
            ExcludeEnvironmentCredential = true,
            ExcludeAzureCliCredential = true,
            ExcludeInteractiveBrowserCredential = false,
            ExcludeManagedIdentityCredential = true,
            ExcludeSharedTokenCacheCredential = true,
            ExcludeVisualStudioCodeCredential = true,
            ExcludeVisualStudioCredential = false,
            //VisualStudioTenantId
        };



        //builder.Logging.AddDebug("aaaaaaaaaaaaaaaaaaaaaaa");
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
