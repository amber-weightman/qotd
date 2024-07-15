namespace Qotd.Api.Extensions;

public static class HttpContextExtensions
{
    public static string? GetIp(this HttpContext context)
    {
        var ipAddress = context.GetServerVariable("HTTP_X_FORWARDED_FOR") ?? context.Connection.RemoteIpAddress?.ToString();
        var ipAddressWithoutPort = ipAddress?.Split(':')[0];
        //ipAddressWithoutPort = "159.196.170.108"; // uncomment for local testing
        
        return ipAddressWithoutPort;
    }
}
