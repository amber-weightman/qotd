using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Qotd.Api.Controllers;
using Qotd.Api.Filters;
using Qotd.Application.Interfaces;
using Qotd.Application.Models;

namespace Qotd.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase, IMetadataController
{
    private readonly IQuestionService _questionService;
    
    public Metadata? Metadata { get; set; }

    public QuestionController(IQuestionService questionService, IHttpContextAccessor httpContextAccessor)
    {
        _questionService = questionService;
    }

    [MetadataCookiesFilter]
    [OutputCache(Duration = 10)]
    [HttpGet("setup")]
    public async Task Setup(CancellationToken cancellationToken)
    {
        var response = await _questionService.Setup(Metadata, cancellationToken);
        Metadata = response;
    }

    [MetadataCookiesFilter]
    [HttpGet("generate-question")]
    public async Task<string> GenerateQuestion(CancellationToken cancellationToken)
    {
        var response = await _questionService.GenerateQuestion(Metadata, cancellationToken);

        Metadata = response.Metadata;

        return response.RunId; // TODO fe shouldn't know about this
    }

    [MetadataCookiesFilter]
    [HttpGet("get-question/{run-id}")]
    public async Task<string> GetQuestion([FromRoute]string runId, CancellationToken cancellationToken)
    {
        var response = await _questionService.GetQuestion(Metadata, cancellationToken);

        Metadata = response.Metadata;

        return response.Question;
    }
}
