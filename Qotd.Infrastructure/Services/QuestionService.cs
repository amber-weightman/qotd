using Qotd.Application.Interfaces;
using Qotd.Application.Models;
using Qotd.Infrastructure.AI.Models;
using Qotd.Infrastructure.ChatGpt;
using static System.Net.Mime.MediaTypeNames;

namespace Qotd.Infrastructure.Services;

internal record QuestionService : IQuestionService
{
    private readonly IAIClient _client;
    public QuestionService(IAIClient client)
    {
        _client = client;
    }

    public async Task<Metadata> Setup(Metadata? metadata, CancellationToken cancellationToken)
    {
        var assistantId = metadata is not null && metadata.Values.ContainsKey("AssistantId") ? metadata.Values["AssistantId"] : null;
        var threadId = metadata is not null && metadata.Values.ContainsKey("ThreadId") ? metadata.Values["ThreadId"] : null;

        var response = await _client.SetupThread(assistantId, threadId, cancellationToken);

        return new Metadata
        {
            Values = new Dictionary<string, string>
                {
                    { "AssistantId", response.AssistantId },
                    { "ThreadId", response.ThreadId },
                    //{ "RunId", response.RunId }
                }
        };
    }

    public async Task<RunResponse> GenerateQuestion(Metadata metadata, CancellationToken cancellationToken)
    {
        var assistantId = metadata.Values.ContainsKey("AssistantId") ? metadata.Values["AssistantId"] : null;
        var threadId = metadata.Values.ContainsKey("ThreadId") ? metadata.Values["ThreadId"] : null;

        if (assistantId is null || threadId is null)
        {
            throw new ArgumentException("Metadata does not contain the expected values");
        }

        var response = await _client.RequestQuestion(assistantId, threadId, cancellationToken);

        return new RunResponse
        {
            RunId = response.RunId,
            Metadata = new Metadata
            {
                Values = new Dictionary<string, string>
                {
                    { "AssistantId", response.AssistantId },
                    { "ThreadId", response.ThreadId }
                }
            }
        };
    }

    public async Task<QuestionResponse> GetQuestion(Metadata metadata, string runId, CancellationToken cancellationToken)
    {
        var threadId = metadata.Values.ContainsKey("ThreadId") ? metadata.Values["ThreadId"] : null;

        if (threadId is null)
        {
            throw new ArgumentException("Metadata does not contain the expected values");
        }

        var test = await _client.FetchQuestion(threadId, runId, cancellationToken);

        return new QuestionResponse
        {
            Question = test.Question ?? Constants.Common.DefaultQuestion,
            Metadata = new Metadata
            {
                Values = new Dictionary<string, string>
                {
                    { "ThreadId", test.ThreadId }
                }
            }
        };
    }
}
