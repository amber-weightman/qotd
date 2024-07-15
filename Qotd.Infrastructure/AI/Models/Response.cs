namespace Qotd.Infrastructure.AI.Models;

internal record ResponseBase
{
    public string AssistantId { get; init; } = null!;

    public string ThreadId { get; init; } = null!;

    public string? RunId { get; init; }
}

internal sealed record Response : ResponseBase
{
    public string? Question { get; init; }
}