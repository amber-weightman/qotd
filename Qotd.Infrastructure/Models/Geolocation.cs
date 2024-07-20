using System.Diagnostics.CodeAnalysis;

namespace Qotd.Infrastructure.Models;

[ExcludeFromCodeCoverage]

internal sealed record Geolocation
{
    public string? Country { get; init; }

    public string? IP { get; init; }
}
