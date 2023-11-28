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

    public async Task<string> GetQuestion(CancellationToken cancellationToken)
    {
        var test = await _client.Testing(cancellationToken);
        return test ?? "What's the answer to life, the universe and everything?";
    }
}
