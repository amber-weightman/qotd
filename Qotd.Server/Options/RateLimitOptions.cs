using System.Diagnostics.CodeAnalysis;

namespace Qotd.Api.Options;

[ExcludeFromCodeCoverage]
internal static class RateLimitOptions
{
    public const string DefaultPolicyName = "DefaultRateLimitPolicy";
}
