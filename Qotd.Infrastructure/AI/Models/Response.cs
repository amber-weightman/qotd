using System.Diagnostics.CodeAnalysis;

namespace Qotd.Infrastructure.AI.Models;

[ExcludeFromCodeCoverage]
internal record ResponseBase
{
    public string AssistantId { get; init; } = null!;

    public string ThreadId { get; init; } = null!;

    public string? RunId { get; init; }
}

[ExcludeFromCodeCoverage]
internal sealed record Response : ResponseBase
{
    public string? Question { get; init; }
}