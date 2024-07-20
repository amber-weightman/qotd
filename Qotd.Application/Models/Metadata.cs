using System.Diagnostics.CodeAnalysis;

namespace Qotd.Application.Models;

[ExcludeFromCodeCoverage]
public record Metadata
{
    public Dictionary<string, string> Values { get; init; }
}
