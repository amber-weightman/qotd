using System.Diagnostics.CodeAnalysis;

namespace Qotd.Application.Enums;

/// <summary>
/// Types of client authentication
/// </summary>
public enum AuthenticationClientType
{
    /// <summary>
    /// Unknown or unauthenticated.
    /// </summary>
    Unknown,

    /// <summary>
    /// The default API Key used by the public website.
    /// Not really a secret, not much better than unauthenticated.
    /// </summary>
    DefaultPublic,

    /// <summary>
    /// Secure private API access.
    /// </summary>
    Api,

    /// <summary>
    /// Secure private ADMIN API access.
    /// </summary>
    Admin
}

/// <summary>
/// Temporary solution until login and API key creation is supported
/// </summary>
[ExcludeFromCodeCoverage]
public static class ClientNames
{
    public const string Default = "DEFAULT";
    public const string Anonymous = "ANONYMOUS";
    public const string Admin = "ADMIN";
}
