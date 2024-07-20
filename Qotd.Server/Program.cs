using Qotd.Api.Options;
using Qotd.Api.Startup;
using Qotd.Infrastructure.Startup;
using Qotd.Server.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.AddRateLimit();

builder.Configuration.AddUserSecrets<QuestionController>();
builder.Configuration.ConfigureAzure();

builder.Services.ConfigureApi();

builder.Services.AddControllers();

builder.Services.ConfigureSwagger();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.ConfigureInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks();

var app = builder.Build();

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
// TODO add to Swagger
app.MapHealthChecks("/health")
    .RequireHost("*:7011", 
        "*:5001",
        "*:5173",
        "questionoftheday.azurewebsites.net:*", 
        "www.questionoftheday.azurewebsites.net:*",
        "builtbyamber.com:*",
        "www.builtbyamber.com:*"
    );

await app.RunAsync();
