using OpenAI;
using OpenAI.Assistants;
using OpenAI.Threads;
using Qotd.Infrastructure.AI;
using Qotd.Infrastructure.AI.Models;
using System.Threading;

namespace Qotd.Infrastructure.ChatGpt;

internal interface IAIClient
{
    Task<ResponseBase> SetupThread(CancellationToken cancellationToken);
    Task<ResponseBase> SetupThread(string? assistantId, string? threadId, CancellationToken cancellationToken);
    Task<ResponseBase> RequestQuestion(string assistantId, string threadId, CancellationToken cancellationToken);
    Task<Response> FetchQuestion(string threadId, string runId, CancellationToken cancellationToken);
}

internal record AIClient : IAIClient
{
    private static readonly string Model = "gpt-3.5-turbo";

    private readonly OpenAIClient _client;

    public AIClient(OpenAIClient client)
    {
        _client = client;
    }

    private async Task<AssistantResponse> CreateAssistant(string name, string instructions, CancellationToken cancellationToken)
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
        return messageList;
    }

    // https://github.com/RageAgainstThePixel/OpenAI-DotNet#assistants

    //private async Task<RequestOptions> ValidateOptions(RequestOptions? options, CancellationToken cancellationToken)
    //{
    //    RequestOptions newOptions;
    //    if (options is null || options.RunId is null)
    //    {
    //        var setupResponse = await SetupThread(options, cancellationToken);
    //        newOptions = new RequestOptions
    //        {
    //            AssistantId = setupResponse.AssistantId,
    //            ThreadId = setupResponse.ThreadId,
    //            RunId = setupResponse.RunId
    //        };
    //    }
    //    else
    //    {
    //        newOptions = options;
    //    }
    //    if (newOptions.ThreadId is null || newOptions.RunId is null)
    //    {
    //        throw new ApplicationException();
    //    }
    //    return newOptions;
    //}

    // https://github.com/RageAgainstThePixel/OpenAI-DotNet#assistants

    /// <summary>
    /// Create a new assistant (if not already existing) and a new thread (if not already existing)
    /// </summary>
    public async Task<ResponseBase> SetupThread(string? assistantId, string? threadId, CancellationToken cancellationToken)
    {
        string assistantId2 = assistantId ?? (await CreateAssistant(Constants.Common.AssistantName, InstructionsBuilder.GetInstructions(), cancellationToken)).Id;
        string threadId2 = threadId ?? (await CreateThread(cancellationToken)).Id;

        return new ResponseBase
        {
            AssistantId = assistantId2,
            ThreadId = threadId2,
            RunId = null
        };
    }

    /// <summary>
    /// Create a new assistant and a new thread
    /// </summary>
    public async Task<ResponseBase> SetupThread(CancellationToken cancellationToken)
    {
        string assistantId = (await CreateAssistant(Constants.Common.AssistantName, InstructionsBuilder.GetInstructions(), cancellationToken)).Id;
        string threadId = (await CreateThread(cancellationToken)).Id;

        return new ResponseBase
        {
            AssistantId = assistantId,
            ThreadId = threadId,
            RunId = null
        };
    }

    /// <summary>
    /// Create a new run (initiate requesting a question)
    /// </summary>
    public async Task<ResponseBase> RequestQuestion(string assistantId, string threadId, CancellationToken cancellationToken)
    {
        string runId;        
        try
        {
            runId = (await CreateRun(threadId, assistantId, cancellationToken)).Id;
        }
        catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Assistant and/or thread may no longer exist on the server, so setup a fresh thread and run that instead

            var newNewOptions = await SetupThread(cancellationToken);
            threadId = newNewOptions.ThreadId; // TODO shouldn't be re-assigning incoming args
            assistantId = newNewOptions.AssistantId;
            runId = (await CreateRun(threadId, newNewOptions.AssistantId, cancellationToken)).Id;
        }

        return new ResponseBase
        {
            ThreadId = threadId,
            AssistantId = assistantId,
            RunId = runId
        };
    }

    public async Task<Response> FetchQuestion(string threadId, string runId, CancellationToken cancellationToken)
    {
        RunResponse run = await GetRun(threadId, runId, cancellationToken);
        while ((run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress) && !cancellationToken.IsCancellationRequested)
        {
            Thread.Sleep(500);
            run = await GetRun(threadId, runId, cancellationToken);
        }

        var messages = await GetMessages(threadId, cancellationToken);

        return new Response
        {
            Question = messages.Items.FirstOrDefault()?.Content?.FirstOrDefault()?.Text?.Value,
            ThreadId = threadId,
            RunId = runId
        };
    }
}
