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
        var options = metadata is null ? null : new RequestOptions
        {
            AssistantId = metadata.Values.ContainsKey("AssistantId") ? metadata.Values["AssistantId"] : null,
            ThreadId = metadata.Values.ContainsKey("ThreadId") ? metadata.Values["ThreadId"] : null
        };

        var response = await _client.SetupThread(options.AssistantId, options.ThreadId, cancellationToken);

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
        var options = metadata is null ? null : new RequestOptions
        {
            AssistantId = metadata.Values.ContainsKey("AssistantId") ? metadata.Values["AssistantId"] : null,
            ThreadId = metadata.Values.ContainsKey("ThreadId") ? metadata.Values["ThreadId"] : null
        };

        var response = await _client.RequestQuestion(options.AssistantId, options.ThreadId, cancellationToken);

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

    public async Task<QuestionResponse> GetQuestion(Metadata metadata, CancellationToken cancellationToken)
    {
        var options = metadata is null ? null : new RequestOptions
        {
            //AssistantId = metadata.Values.ContainsKey("AssistantId")  ? metadata.Values["AssistantId"] : null,
            ThreadId = metadata.Values.ContainsKey("ThreadId") ? metadata.Values["ThreadId"] : null,
            RunId = metadata.Values.ContainsKey("RunId") ? metadata.Values["RunId"] : null
        };

        var test = await _client.FetchQuestion(options.ThreadId, options.RunId, cancellationToken);

        return new QuestionResponse
        {
            Question = test.Question ?? Constants.Common.DefaultQuestion,
            Metadata = new Metadata
            {
                Values = new Dictionary<string, string>
                {
                    //{ "AssistantId", test.AssistantId },
                    { "ThreadId", test.ThreadId },
                    //{ "RunId", test.RunId }
                }
            }
        };
    }
}
