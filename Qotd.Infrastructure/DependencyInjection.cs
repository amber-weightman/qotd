using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using Qotd.Application.Interfaces;
using Qotd.Infrastructure.ChatGpt;
using Qotd.Infrastructure.Services;

namespace Qotd.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IQuestionService, QuestionService>();
        services.AddTransient<IAIClient, AIClient>();

        var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        //https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        services.AddHttpClient<OpenAI.OpenAIClient>("AIHttpClient");
        //.AddPolicyHandler(GetRetryPolicy())
        //.AddPolicyHandler(GetCircuitBreakerPolicy());

        // https://github.com/RageAgainstThePixel/OpenAI-DotNet#use-system-environment-variables
        services.AddTransient<OpenAIClient>(ctx =>
        {
            var auth = new OpenAIAuthentication(configBuilder.GetValue<string>("OpenAI:apiKey"), configBuilder.GetValue<string>("OpenAI:organization"));

            var clientFactory = ctx.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient("AIHttpClient");

            return new OpenAIClient(auth, null, httpClient); 
        });


        

        return services;
    }
}