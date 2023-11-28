using OpenAI;
using OpenAI.Assistants;
using OpenAI.Threads;
using System.Threading;

namespace Qotd.Infrastructure.ChatGpt;

internal interface IAIClient
{
    Task<AssistantResponse> CreateAssistant(string name, string instructions, CancellationToken cancellationToken);
    Task<string?> Testing(CancellationToken cancellationToken);
}

internal record AIClient : IAIClient
{
    private static readonly string Model = "gpt-3.5-turbo";

    private readonly OpenAIClient _client;

    public AIClient(OpenAIClient client)
    {
        _client = client;
    }

    public async Task<AssistantResponse> CreateAssistant(string name, string instructions, CancellationToken cancellationToken)
    {
        var request = new CreateAssistantRequest(model: Model, name: name, instructions: instructions);
        var assistant = await _client.AssistantsEndpoint.CreateAssistantAsync(request, cancellationToken);
        if (assistant is null)
        {
            throw new ApplicationException("Failed to create Assistant");
        }
        return assistant;
    }

    private async Task<AssistantResponse> GetAssistant(string assistantId, CancellationToken cancellationToken)
    {
        var assistant = await _client.AssistantsEndpoint.RetrieveAssistantAsync(assistantId, cancellationToken);
        if (assistant is null)
        {
            throw new ApplicationException($"Failed to get Assistant {assistantId}");
        }
        return assistant;
    }

    private async Task DeleteAssistant(string assistantId, CancellationToken cancellationToken)
    {
        var isDeleted = await _client.AssistantsEndpoint.DeleteAssistantAsync(assistantId, cancellationToken);
        if (!isDeleted)
        {
            throw new ApplicationException($"Failed to delete Assistant {assistantId}");
        }
    }

    private async Task<ThreadResponse> CreateThread(CancellationToken cancellationToken)
    {
        var messages = new List<Message> { "give me a question please" };
        var request = new CreateThreadRequest(messages);
        var thread = await _client.ThreadsEndpoint.CreateThreadAsync(request, cancellationToken);
        if (thread is null)
        {
            throw new ApplicationException("Failed to create Thread");
        }
        return thread;
    }

    // More thread messages I haven't brought in...

    private async Task<MessageResponse> CreateMessage(string threadId, CancellationToken cancellationToken)
    {
        var request = new CreateMessageRequest("give me a question please" );
        var message = await _client.ThreadsEndpoint.CreateMessageAsync(threadId, request, cancellationToken);

        if (message is null)
        {
            throw new ApplicationException("Failed to create Message");
        }
        return message;
    }

    private async Task<RunResponse> CreateRun(string threadId, string assistantId, CancellationToken cancellationToken)
    {
        var request = new CreateRunRequest(assistantId, model: Model);
        var run = await _client.ThreadsEndpoint.CreateRunAsync(threadId, request, cancellationToken);
        if (run is null)
        {
            throw new ApplicationException("Failed to create Run");
        }
        return run;
    }

    private async Task<RunResponse> GetRun(string threadId, string runId, CancellationToken cancellationToken)
    {
        var run = await _client.ThreadsEndpoint.RetrieveRunAsync(threadId, runId, cancellationToken);
        if (run is null)
        {
            throw new ApplicationException("Failed to get Run");
        }
        return run;
    }

    private async Task<ListResponse<MessageResponse>> GetMessages(string threadId, CancellationToken cancellationToken)
    {
        var messageList = await _client.ThreadsEndpoint.ListMessagesAsync(threadId, null, cancellationToken);
        if (messageList is null)
        {
            throw new ApplicationException("Failed to get Messages");
        }

        foreach (var message in messageList.Items)
        {
            Console.WriteLine($"{message.Id}: {message.Role}: {message.PrintContent()}");
        }


        return messageList;
    }

    public async Task<string?> Testing(CancellationToken cancellationToken)
    {
        // https://github.com/RageAgainstThePixel/OpenAI-DotNet#assistants

        var assistant = await CreateAssistant("Manager", "You are a manager of a remote team. When you run meetings, you always ask a \"question of the day\" as a fun, non-work-related get-to-know-you and icebreaker question, before the meeting begins. Answer questions with a single question (your answer should not be presented as a list). There should be good variety between each question, day to day and week to week. Sometimes they can be more serious (but still not work-related) and sometimes they can be lighthearted and funny or silly. Questions should not be repeated.", cancellationToken);

        var threadRequest = await CreateThread(cancellationToken);
        var run = await CreateRun(threadRequest.Id, assistant.Id, cancellationToken);
        do
        {
            Thread.Sleep(1000);
            run = await GetRun(threadRequest.Id, run.Id, cancellationToken);
        } while ((run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress) && !cancellationToken.IsCancellationRequested);

        var messages = await GetMessages(threadRequest.Id, cancellationToken);

        return messages.Items?.FirstOrDefault()?.Content?.FirstOrDefault()?.Text?.Value;
    }
}
