namespace Qotd.Infrastructure.AI.Models;

internal record RequestOptions
{
    public string ThreadId { get; init; } = null!;

    public string AssistantId { get; init; } = null!;

    public string? RunId { get; init; }
}
