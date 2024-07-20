using Qotd.Application.Enums;

namespace Qotd.Application.Models;

public record AuthenticationResponse
{
    public string? ClientName { get; init; }

    public AuthenticationClientType ClientType { get; init; }

    public bool IsAuthenticated { get; init; }
}
