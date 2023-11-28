namespace Qotd.Application.Interfaces;

public interface IQuestionService
{
    public Task<string> GetQuestionAsync();
}
