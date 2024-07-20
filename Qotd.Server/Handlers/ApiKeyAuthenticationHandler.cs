using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Qotd.Api.Options;
using Qotd.Application.Interfaces;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Qotd.Api.Handlers;

internal sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IClientService _clientService;

    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, IClientService clientService) : base(options, logger, encoder)
    {
        _clientService = clientService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var apiKey) && apiKey.Count == 1)
        {
            return await HandleClientKeyAuth(apiKey);
        }

        Logger.LogWarning("An unauthenticated API request was received");
        return AuthenticateResult.Fail("Invalid parameters");
    }

    private async Task<AuthenticateResult> HandleClientKeyAuth(string apiKey)
    {
        var client = _clientService.AuthenticateClient(apiKey);

        if (!client.IsAuthenticated)
        {
            Logger.LogWarning($"An API request was received with an invalid API key: {apiKey}");
            return AuthenticateResult.Fail("Invalid parameters");
        }

        Logger.BeginScope("{ClientName}", client.ClientName);
        Logger.LogInformation("Client authenticated");

        var claims = new[] { new Claim(ClaimTypes.Name, client.ClientName) };
        var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationOptions.DefaultScheme);
        var identities = new List<ClaimsIdentity> { identity };
        var principal = new ClaimsPrincipal(identities);
        var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.DefaultScheme);

        return AuthenticateResult.Success(ticket);

    }
}
