using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.CodeAnalysis;

namespace Qotd.Api.Options;

[ExcludeFromCodeCoverage]
internal sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ClientKey";
    public const string HeaderName = "x-api-key";
}
