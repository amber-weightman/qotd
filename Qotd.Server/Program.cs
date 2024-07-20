using Microsoft.AspNetCore.HttpOverrides;
using Qotd.Api;
using Qotd.Api.Options;
using Qotd.Api.Startup;
using Qotd.Application;
using Qotd.Infrastructure;
using Qotd.Server.Controllers;

var builder = WebApplication.CreateBuilder(args);


// forward headers configuration for reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options => {
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.AddRateLimit();

builder.Configuration.AddUserSecrets<QuestionController>();
builder.Configuration.ConfigureAzure();

builder.Services.ConfigureApi();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureSwagger();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.ConfigureApplication();
builder.Services.ConfigureInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks();

var app = builder.Build();

//app.UseAzureAppConfiguration();


app.Logger.LogInformation("Adding Routes");

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimit();

app.MapControllers().RequireRateLimiting(RateLimitOptions.DefaultPolicyName);

app.MapFallbackToFile("/index.html");

// TODO gotta work out which hosts I'll actually be using
// TODO add to swagger
app.MapHealthChecks("/health")
    .RequireHost("*:7011", 
        "*:5001",
        "*:5173",
        "questionoftheday.azurewebsites.net:*", 
        "www.questionoftheday.azurewebsites.net:*",
        "builtbyamber.com:*",
        "www.builtbyamber.com:*"
    );
//.RequireAuthorization();

await app.RunAsync();
