using Microsoft.Extensions.DependencyInjection;
using Qotd.Application.Interfaces;
using Qotd.Infrastructure.ChatGpt;
using Qotd.Infrastructure.Services;

namespace Qotd.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IQuestionService, QuestionService>();
        services.AddTransient<IOpenAIClient, OpenAIClient>();

        return services;
    }
}