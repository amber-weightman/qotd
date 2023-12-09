using Microsoft.AspNetCore.Mvc;
using Qotd.Application.Models;

namespace Qotd.Server.Controllers;

[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    private readonly string _secretsTest;
    private readonly string _org;

    public Metadata? Metadata { get; set; }

    public DebugController(IConfiguration configuration)
    {
        _secretsTest = configuration["SecretsTest"];
        _org = configuration["OpenAI:organization"];
    }

    [HttpGet("test")]
    public async Task<string> Test(CancellationToken cancellationToken)
    {
        
        return $"{_secretsTest} {_org}";
    }

    
}
