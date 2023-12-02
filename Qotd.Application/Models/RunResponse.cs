namespace Qotd.Application.Models;

public record RunResponse
{
    public string RunId { get; init; } = null!;

    public Metadata Metadata { get; init; } = null!;
}
