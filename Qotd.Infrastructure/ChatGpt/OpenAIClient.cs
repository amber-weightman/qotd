using OpenAI;
using OpenAI.Assistants;
using OpenAI.Threads;

namespace Qotd.Infrastructure.ChatGpt;

internal interface IOpenAIClient
{
    Task<string> Testing();
}

internal record OpenAIClient : IOpenAIClient
{
    public async Task<string> Testing()
    {
    //https://github.com/RageAgainstThePixel/OpenAI-DotNet#authentication
        var api = new OpenAI.OpenAIClient();
        var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(
            new CreateAssistantRequest(
                name: "Math Tutor",
                instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
                model: "gpt-4-1106-preview"));
        var messages = new List<Message> { "I need to solve the equation `3x + 11 = 14`. Can you help me?" };
        var threadRequest = new CreateThreadRequest(messages);
        var run = await assistant.CreateThreadAndRunAsync(threadRequest);
        Console.WriteLine($"Created thread and run: {run.ThreadId} -> {run.Id} -> {run.CreatedAt}");

        return "";
    }
}
