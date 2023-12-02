using Qotd.Infrastructure.AI.Models;

namespace Qotd.Infrastructure.AI;

internal static class InstructionsBuilder
{
    public static string GetInstructions()
    {
        return Constants.Common.AssistantInstructions;
    }
}
