namespace Qotd.Application.Models;

public record RunResponse
{
    public string QuestionId { get; init; } = null!;

    public Metadata Metadata { get; init; } = null!;
}
