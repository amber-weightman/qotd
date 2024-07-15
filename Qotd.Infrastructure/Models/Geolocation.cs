namespace Qotd.Infrastructure.Models;

internal sealed record Geolocation
{
    public string? Country { get; init; }

    public string? IP { get; init; }
}
