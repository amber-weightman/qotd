using Qotd.Application.Interfaces;
using Qotd.Infrastructure.ChatGpt;

namespace Qotd.Infrastructure.Services;

internal record QuestionService : IQuestionService
{
    private readonly IAIClient _client;
    public QuestionService(IAIClient client)
    {
        _client = client;
    }

    public async Task<string> GetQuestionAsync()
    {
        var test = await _client.Testing();
        return "What's the answer to life, the universe and everything? " + Guid.NewGuid();
    }
}
