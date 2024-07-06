using Microsoft.AspNetCore.Mvc;
using Qotd.Application.Models;

namespace Qotd.Server.Controllers;

/// <summary>
/// Un-comment for testing locally (and delete when app is more stable)
/// </summary>
[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    //private readonly string _org;
    //private readonly string _org2;

    //public Metadata? Metadata { get; set; }

    //public DebugController(IConfiguration configuration)
    //{
    //    _org = configuration["OpenAI:organization"];
    //    _org2 = configuration["ConnectionStrings:AppConfig"];
    //}

    ///// <summary>
    ///// Un-comment for testing locally (and delete when app is more stable)
    ///// </summary>
    //[HttpGet("test")]
    //public async Task<string> Test(CancellationToken cancellationToken)
    //{
    //    return $"{_org} {_org2}";
    //}

    
}
