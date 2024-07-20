using Qotd.Application.Models;
using System.Diagnostics.CodeAnalysis;

namespace Qotd.Application.Interfaces;

public interface IClientService
{
    public AuthenticationResponse AuthenticateClient(string apiKey);

    public Task InvalidateApiKey(string apiKey);
}
