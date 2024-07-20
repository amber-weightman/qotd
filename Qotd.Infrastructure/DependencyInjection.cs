using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using Qotd.Application.Interfaces;
using Qotd.Infrastructure.ChatGpt;
using Qotd.Infrastructure.Clients;
using Qotd.Infrastructure.Services;
using System.Diagnostics.CodeAnalysis;

namespace Qotd.Infrastructure;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddTransient<IQuestionService, QuestionService>();
        services.AddTransient<IAiClient, AiClient>();

        //https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        services.AddHttpClient<OpenAI.OpenAIClient>("AIHttpClient");
        //.AddPolicyHandler(GetRetryPolicy())
        //.AddPolicyHandler(GetCircuitBreakerPolicy());

        // https://github.com/RageAgainstThePixel/OpenAI-DotNet#use-system-environment-variables
        services.AddTransient<OpenAIClient>(ctx =>
        {
            var auth = new OpenAIAuthentication(
                configuration.GetValue<string>("OpenAI:apiKey"),
                configuration.GetValue<string>("OpenAI:organization"));

            var clientFactory = ctx.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient("AIHttpClient");

            return new OpenAIClient(auth, null, httpClient);
        });

        services.AddHttpClient<IIpApiClient, IpApiClient>();

        services.AddScoped<IApiKeyService, ApiKeyService>();

        services.AddScoped<IClientService, ClientService>(ctx =>
        {
            var systemApiKey = configuration.GetValue<string>("Authorization:ClientApiKey");
            if (systemApiKey is null)
            {
                throw new ApplicationException("Could not initialise ClientService");
            }
            return new ClientService(systemApiKey);
        });

        return services;
    }
}