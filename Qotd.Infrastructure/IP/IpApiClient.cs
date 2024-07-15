using Qotd.Infrastructure.IP.Models;
using System.Net.Http.Json;

namespace Qotd.Infrastructure.Clients;

internal interface IIpApiClient
{
    Task<IpApiLiteResponse?> Geolocate(string? ipAddress, CancellationToken cancellationToken);
}

internal sealed record IpApiClient(HttpClient httpClient) : IIpApiClient
{
    private const string BASE_URL = "http://ip-api.com";
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IpApiLiteResponse?> Geolocate(string? ipAddress, CancellationToken cancellationToken)
    {
        var route = $"{BASE_URL}/json/{ipAddress}?lang=en&fields=city,country,countryCode,message,query,region,regionName,status,timezone";
        var response = await _httpClient.GetFromJsonAsync<IpApiLiteResponse>(route, cancellationToken);
        return response;

        // TODO handle rate limiting check: https://ip-api.com/docs/api:json
    }
}
