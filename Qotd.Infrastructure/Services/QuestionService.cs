using Qotd.Application.Interfaces;

namespace Qotd.Infrastructure.Services;

internal record QuestionService : IQuestionService
{
    public Task<string> GetQuestionAsync()
    {
        return Task.FromResult("What's the answer to life, the universe and everything? " + Guid.NewGuid());   
    }
}
