namespace Qotd.Application.Models;

public record QuestionResponse
{
    public string Question { get; init; } = null!;

    public Metadata Metadata { get; init; } = null!;
}
