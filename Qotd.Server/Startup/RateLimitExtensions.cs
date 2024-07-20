using Microsoft.AspNetCore.HttpOverrides;
using Qotd.Api.Options;
using Qotd.Application.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Threading.RateLimiting;

namespace Qotd.Api.Startup;

[ExcludeFromCodeCoverage]
public static class RateLimitWebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddRateLimit(this WebApplicationBuilder builder)
    {
        // forward headers configuration for reverse proxy
        builder.Services.Configure<ForwardedHeadersOptions>(options => {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        builder.Services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            limiterOptions.AddPolicy(policyName: RateLimitOptions.DefaultPolicyName, partitioner: httpContext =>
            {
                var user = httpContext.User?.Identity?.Name ?? ClientNames.Anonymous;

                if (user == ClientNames.Admin)
                {
                    return RateLimitPartition.GetNoLimiter(user);
                }

                if (user == ClientNames.Default)
                {
                    return RateLimitPartition.GetTokenBucketLimiter(user, _ =>
                        new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 32,
                            QueueLimit = 0,
                            ReplenishmentPeriod = TimeSpan.FromHours(12),
                            TokensPerPeriod = 32,
                            AutoReplenishment = true
                        });
                }

                return RateLimitPartition.GetTokenBucketLimiter(user, _ =>
                    new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 16,
                        QueueLimit = 0,
                        ReplenishmentPeriod = TimeSpan.FromHours(12),
                        TokensPerPeriod = 16,
                        AutoReplenishment = true
                    });

            });
            //limiterOptions.OnRejected = async (context, token) =>
            //{
            //    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            //    await context.HttpContext.Response.WriteAsync("Too many requests. Please try later again... ", cancellationToken: token);
            //};
        });

        return builder;
    }
}

public static class RateLimitWebApplicationExtensions
{
    public static WebApplication UseRateLimit(this WebApplication app)
    {
        app.UseRateLimiter();
        return app;
    }
}