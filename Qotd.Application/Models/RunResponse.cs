using System.Diagnostics.CodeAnalysis;

namespace Qotd.Application.Models;

[ExcludeFromCodeCoverage]
public record RunResponse
{
    public string QuestionId { get; init; } = null!;

    public Metadata Metadata { get; init; } = null!;
}
