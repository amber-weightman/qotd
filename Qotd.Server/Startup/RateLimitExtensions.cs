using Qotd.Api.Options;
using Qotd.Application.Enums;
using System.Threading.RateLimiting;

namespace Qotd.Api.Startup;

public static class RateLimitWebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddRateLimit(this WebApplicationBuilder builder)
    {
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
                            TokenLimit = 15,
                            QueueLimit = 5,
                            ReplenishmentPeriod = TimeSpan.FromHours(12),
                            TokensPerPeriod = 15,
                            AutoReplenishment = true
                        });
                }

                return RateLimitPartition.GetTokenBucketLimiter(user, _ =>
                    new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 5,
                        QueueLimit = 5,
                        ReplenishmentPeriod = TimeSpan.FromHours(12),
                        TokensPerPeriod = 5,
                        AutoReplenishment = true
                    });

            });
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