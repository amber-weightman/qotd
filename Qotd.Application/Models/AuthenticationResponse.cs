using Qotd.Application.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Qotd.Application.Models;

[ExcludeFromCodeCoverage]
public record AuthenticationResponse
{
    public string? ClientName { get; init; }

    public AuthenticationClientType ClientType { get; init; }

    public bool IsAuthenticated { get; init; }
}
