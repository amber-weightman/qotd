namespace Qotd.Infrastructure.IP.Models;

internal record IpApiLiteResponse
{
    /// <summary>
    /// City
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    /// Country name
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    /// Two-letter country code ISO 3166-1 alpha-2
    /// </summary>
    public string? CountryCode { get; init; }

    /// <summary>
    /// Region/state short code (FIPS or ISO)
    /// </summary>
    public string? Region { get; init; }

    /// <summary>
    /// Region/state
    /// </summary>
    public string? RegionName { get; init; }

    /// <summary>
    /// Timezone (tz)
    /// </summary>
    public string? Timezone { get; init; }

    /// <summary>
    /// success or fail
    /// </summary>    
    public string? Status { get; init; }

    /// <summary>
    /// included only when status is fail
    /// Can be one of the following: private range, reserved range, invalid query
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// IP used for the query
    /// </summary>
    public string? Query { get; init; }
}
