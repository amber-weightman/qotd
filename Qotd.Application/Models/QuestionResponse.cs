using System.Diagnostics.CodeAnalysis;

namespace Qotd.Application.Models;

[ExcludeFromCodeCoverage]
public record QuestionResponse
{
    public string Question { get; init; } = null!;

    public Metadata Metadata { get; init; } = null!;
}
