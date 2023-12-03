namespace Qotd.Infrastructure.AI;

internal static class InstructionsBuilder
{
    public static string GetInstructions()
    {
        return Constants.Common.AssistantInstructions;
    }

    public static string GetPrompt()
    {
        return Constants.Common.AssistantPrompt;
    }
}
