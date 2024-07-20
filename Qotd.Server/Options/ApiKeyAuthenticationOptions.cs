using Microsoft.AspNetCore.Authentication;

namespace Qotd.Api.Options;

internal sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ClientKey";
    public const string HeaderName = "x-api-key";
}
