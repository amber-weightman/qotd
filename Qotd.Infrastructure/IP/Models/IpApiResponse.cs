using System.Text.Json.Serialization;

namespace Qotd.Infrastructure.IP.Models;

internal sealed record IpApiResponse : IpApiLiteResponse
{
    /// <summary>
    /// Continent name
    /// </summary>
    public string? Continent { get; set; }

    /// <summary>
    /// Two-letter continent code
    /// </summary>
    public string? ContinentCode { get; set; }

    /// <summary>
    /// District (subdivision of city)
    /// </summary>
    public string? District { get; set; }

    /// <summary>
    /// Postal code
    /// </summary>
    [JsonPropertyName("zip")]
    public string? PostCode { get; set; }

    public double? Lat { get; set; }

    public double? Lon { get; set; }

    /// <summary>
    /// Timezone UTC DST offset in seconds
    /// </summary>
    public int? Offset { get; set; }

    /// <summary>
    /// National currency
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// ISP name
    /// </summary>
    public string? Isp { get; set; }

    /// <summary>
    /// Organisation name
    /// </summary>
    public string? Org { get; set; }
}
