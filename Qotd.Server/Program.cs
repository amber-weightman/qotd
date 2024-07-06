using Qotd.Application;
using Qotd.Api;
using Qotd.Infrastructure;
using Qotd.Server.Controllers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<QuestionController>();
builder.Configuration.ConfigureAzure();
//builder.Services.AddAzureAppConfiguration();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

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

app.MapControllers();

app.MapFallbackToFile("/index.html");

// TODO gotta work out which hosts I'll actually be using
// TODO add to swagger
app.MapHealthChecks("/health")
    .RequireHost("*:7011", 
        "*:5001",
        "*:5173",
        "questionoftheday.azurewebsites.net:*", 
        "www.questionoftheday.azurewebsites.net:*",
        "builtbyamber.com:*"
    );
//.RequireAuthorization();

app.Run();
