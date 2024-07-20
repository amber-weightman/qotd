using Azure.Identity;
using Microsoft.OpenApi.Models;
using Qotd.Api.Handlers;
using Qotd.Api.Options;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Qotd.Api.Startup;

/// <summary>
/// Dependency Injection extensions for Qotd.Api
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureApi(this IServiceCollection services)
    {
        services
            .AddScoped<ApiKeyAuthenticationHandler>();

        services.AddAuthentication()
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, null);

        return services;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(setup =>
        {
            setup.AddSecurityDefinition(ApiKeyAuthenticationOptions.DefaultScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = ApiKeyAuthenticationOptions.HeaderName,
                Type = SecuritySchemeType.ApiKey
            });

            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = ApiKeyAuthenticationOptions.DefaultScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            setup.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });
        return services;
    }

    public static ConfigurationManager ConfigureAzure(this ConfigurationManager configuration)
    {
        configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(
                    configuration["ConnectionStrings:AppConfig"])
                    .ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(new DefaultAzureCredential());
                    });
        });
        // If this causes issues running locally, make sure there are no "not logged in" warnings

        return configuration;
    }
}
