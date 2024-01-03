using OpenAI;
using OpenAI.Assistants;
using OpenAI.Threads;
using Qotd.Infrastructure.AI;
using Qotd.Infrastructure.AI.Models;
using Qotd.Infrastructure.Exceptions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Qotd.Infrastructure.ChatGpt;

internal interface IAIClient
{
    Task<ResponseBase> SetupThread(CancellationToken cancellationToken);
    Task<ResponseBase> SetupThread(string? assistantId, string? threadId, CancellationToken cancellationToken);
    Task<ResponseBase> RequestQuestion(string assistantId, string threadId, CancellationToken cancellationToken);
    Task<Response> FetchQuestion(string threadId, string runId, CancellationToken cancellationToken);
    Task Delete(string assistantId, string threadId, CancellationToken cancellationToken);
}

internal record AIClient : IAIClient
{
    private static readonly string Model = Constants.GptModel.GPT3_5_Turbo;

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
            throw new AIException("Failed to create Assistant");
        }
        return assistant;
    }

    private async Task<ThreadResponse> CreateThread(CancellationToken cancellationToken)
    {
        var messages = new List<Message> { InstructionsBuilder.GetPrompt() };
        var request = new CreateThreadRequest(messages);
        var thread = await _client.ThreadsEndpoint.CreateThreadAsync(request, cancellationToken);
        if (thread is null)
        {
            throw new AIException("Failed to create Thread");
        }
        return thread;
    }

    private async Task<RunResponse> CreateRun(string threadId, string assistantId, CancellationToken cancellationToken)
    {
        var request = new CreateRunRequest(assistantId, model: Model);
        var run = await _client.ThreadsEndpoint.CreateRunAsync(threadId, request, cancellationToken);
        if (run is null)
        {
            throw new AIException("Failed to create Run");
        }
        return run;
    }

    private async Task<RunResponse> GetRun(string threadId, string runId, CancellationToken cancellationToken)
    {
        var run = await _client.ThreadsEndpoint.RetrieveRunAsync(threadId, runId, cancellationToken);
        if (run is null)
        {
            throw new AIException("Failed to get Run");
        }
        return run;
    }

    private async Task<RunResponse> GetLatestRun(string threadId, CancellationToken cancellationToken)
    {
        var request = new ListQuery { Limit = 1, Order = SortOrder.Descending };
        var runs = await _client.ThreadsEndpoint.ListRunsAsync(threadId, request, cancellationToken);
        if (runs is null)
        {
            throw new AIException("Failed to get Run");
        }
        return runs.Items?.SingleOrDefault();
    }

    private async Task<ListResponse<MessageResponse>> GetMessages(string threadId, CancellationToken cancellationToken)
    {
        var messageList = await _client.ThreadsEndpoint.ListMessagesAsync(threadId, null, cancellationToken);
        if (messageList is null)
        {
            throw new AIException("Failed to get Messages");
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
    //        throw new AIException();
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
        catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.BadRequest && e.Message.Contains("already has an active run"))
        {
            // TODO parse the message more efficiently
            //CreateRunAsync Failed! HTTP status code: BadRequest | Response body: {
            //"error": {
            //  "message": "Thread thread_oKFhhGiG0C6uzzHtew0iA48v already has an active run run_1KzjUHFRWg8N7JlvgW37cLJp.",
            //  "type": "invalid_request_error",
            //  "param": null,
            //  "code": null
            //}

            var run = await GetLatestRun(threadId, cancellationToken);
            if (run.Id is null)
            {
                throw;
            }
            runId = run.Id;
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

    public async Task Delete(string assistantId, string threadId, CancellationToken cancellationToken)
    {
        var threadDeleted = await _client.ThreadsEndpoint.DeleteThreadAsync(threadId, cancellationToken);
        var assistantDeleted = await _client.AssistantsEndpoint.DeleteAssistantAsync(assistantId, cancellationToken);
        if (!threadDeleted && !assistantDeleted)
        {
            throw new AggregateException(
                new AIException($"Failed to delete Assistant {assistantId}"),
                new AIException($"Failed to delete Thread {threadId}"));
        }
        else if (!assistantDeleted)
        {
            throw new AIException($"Failed to delete Assistant {assistantId}");
        }
        else if (!threadDeleted)
        {
            throw new AIException($"Failed to delete Thread {threadId}");
        }
    }

}
