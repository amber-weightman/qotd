using Qotd.Application.Interfaces;
using Qotd.Application.Models;
using Qotd.Infrastructure.ChatGpt;
using Qotd.Infrastructure.Clients;
using Qotd.Infrastructure.Models;

namespace Qotd.Infrastructure.Services;

internal sealed record QuestionService : IQuestionService
{
    private readonly IAIClient _client;
    private readonly IIpApiClient _ipClient;

    public QuestionService(IAIClient client, IIpApiClient ipClient)
    {
        _client = client;
        _ipClient = ipClient;
    }

    public async Task<Metadata> Setup(Metadata? metadata, string? ipAddress, CancellationToken cancellationToken)
    {
        var assistantId = metadata is not null && metadata.Values.TryGetValue("AssistantId", out var a) ? a : null;
        var threadId = metadata is not null && metadata.Values.TryGetValue("ThreadId", out var t) ? t : null;

        Geolocation? geolocation = null;
        if(!string.IsNullOrEmpty(ipAddress))
        {
            var ipApiResponse = await _ipClient.Geolocate(ipAddress, cancellationToken);
            if(!string.IsNullOrEmpty(ipApiResponse?.Country))
            {
                geolocation = new Geolocation { Country = ipApiResponse.Country, IP = ipAddress };
            }
        }

        var response = await _client.SetupThread(assistantId, threadId, geolocation, cancellationToken);

        return new Metadata
        {
            Values = new Dictionary<string, string>
                {
                    { "AssistantId", response.AssistantId },
                    { "ThreadId", response.ThreadId }
                }
        };
    }

    public async Task<RunResponse> GenerateQuestion(Metadata metadata, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        metadata.Values.TryGetValue("AssistantId", out var assistantId);
        metadata.Values.TryGetValue("ThreadId", out var threadId);

        if (string.IsNullOrEmpty(assistantId) || string.IsNullOrEmpty(threadId))
        {
            throw new ArgumentException("Metadata does not contain the expected values");
        }

        var response = await _client.RequestQuestion(assistantId, threadId, cancellationToken);

        return new RunResponse
        {
            QuestionId = response.RunId,
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
        ArgumentNullException.ThrowIfNull(metadata);

        metadata.Values.TryGetValue("ThreadId", out var threadId);

        if (string.IsNullOrEmpty(threadId) || string.IsNullOrEmpty(runId))
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

    public async Task Delete(Metadata metadata, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        metadata.Values.TryGetValue("AssistantId", out var assistantId);
        metadata.Values.TryGetValue("ThreadId", out var threadId);

        if (string.IsNullOrEmpty(assistantId) || string.IsNullOrEmpty(threadId))
        {
            throw new ArgumentException("Metadata does not contain the expected values");
        }

        await _client.Delete(assistantId, threadId, cancellationToken);
    }
}
