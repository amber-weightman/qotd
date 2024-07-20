using Qotd.Application.Enums;
using Qotd.Application.Interfaces;
using Qotd.Application.Models;

namespace Qotd.Infrastructure.Services;

internal sealed record ClientService : IClientService
{
    private readonly string _systemApiKey;

    public ClientService(string systemApiKey)
    {
        _systemApiKey = systemApiKey;
    }

    public AuthenticationResponse AuthenticateClient(string apiKey)
    {
        if (apiKey.Equals(_systemApiKey))
        {
            return new AuthenticationResponse
            {
                ClientName = "DEFAULT",
                ClientType = AuthenticationClientType.DefaultPublic,
                IsAuthenticated = true
            };
        }
        return new AuthenticationResponse
        {
            ClientName = "ANONYMOUS",
            ClientType = AuthenticationClientType.Unknown,
            IsAuthenticated = false
        };
    }

    public Task InvalidateApiKey(string apiKey)
    {
        throw new NotImplementedException();
    }
}
