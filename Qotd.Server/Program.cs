using Azure.Identity;
using Qotd.Application;
using Qotd.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
// AADSTS500200: User account 'amberweightman@hotmail.com' is a personal Microsoft account. Personal Microsoft accounts are not
// supported for this application unless explicitly invited to an organization. Try signing out and signing back in with an organizational account.


var options = new DefaultAzureCredentialOptions()
{

    ExcludeAzurePowerShellCredential = true,
    ExcludeEnvironmentCredential = true,
    ExcludeAzureCliCredential = true,
    ExcludeInteractiveBrowserCredential = false,
    ExcludeManagedIdentityCredential = true,
    ExcludeSharedTokenCacheCredential = true,
    ExcludeVisualStudioCodeCredential = true,
    ExcludeVisualStudioCredential = false,
    //VisualStudioTenantId
};


//if (builder.Environment.IsProduction())
//{
builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential(/*options*/));
//}

//builder.Configuration.AddAzureAppConfiguration(options =>
//{
//    options.Connect(
//        builder.Configuration["ConnectionStrings:AppConfig"])
//            .ConfigureKeyVault(kv =>
//            {
//                kv.SetCredential(new DefaultAzureCredential());
//            });
//});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.ConfigureApplication();
builder.Services.ConfigureInfrastructure();

builder.Services.AddHealthChecks();

var app = builder.Build();

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
