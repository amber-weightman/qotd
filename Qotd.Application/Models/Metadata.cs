namespace Qotd.Application.Models;

public record Metadata
{
    public Dictionary<string, string> Values { get; init; }
}
