using Qotd.Application.Models;

namespace Qotd.Application.Interfaces;

public interface IClientService
{
    public AuthenticationResponse AuthenticateClient(string apiKey);

    public Task InvalidateApiKey(string apiKey);
}
