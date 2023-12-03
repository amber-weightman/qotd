using Microsoft.AspNetCore.Mvc;
using Qotd.Api.Controllers;
using Qotd.Api.Filters;
using Qotd.Application.Interfaces;
using Qotd.Application.Models;

namespace Qotd.Server.Controllers;

[ApiController]
[Route("question")]
public class QuestionController : ControllerBase, IMetadataController
{
    private readonly IQuestionService _questionService;
    
    public Metadata? Metadata { get; set; }

    public QuestionController(IQuestionService questionService, IHttpContextAccessor httpContextAccessor)
    {
        _questionService = questionService;
    }

    /// <summary>
    /// Initialise your personal AI assistant
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MetadataCookiesFilter]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
    [HttpGet("setup")]
    public async Task Setup(CancellationToken cancellationToken)
    {
        var response = await _questionService.Setup(Metadata, cancellationToken);
        Metadata = response;
    }

    /// <summary>
    /// Ask your pesonal AI assistant to think of a question for you
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Id which can be used to retrieve the question</returns>
    [MetadataCookiesFilter]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 2)]
    [HttpGet("generate-question")]
    public async Task<string> GenerateQuestion(CancellationToken cancellationToken)
    {
        var response = await _questionService.GenerateQuestion(Metadata, cancellationToken);

        Metadata = response.Metadata;

        return response.QuestionId;
    }

    /// <summary>
    /// Retrieve the question
    /// </summary>
    /// <param name="questionId">Id which can be used to retrieve the question</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Your very own personal Question of the Day</returns>
    [MetadataCookiesFilter]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 2)]
    [HttpGet("get-question/{questionId}")]
    public async Task<string> GetQuestion([FromRoute]string questionId, CancellationToken cancellationToken)
    {
        var response = await _questionService.GetQuestion(Metadata, questionId, cancellationToken);

        Metadata = response.Metadata;

        return response.Question;
    }

    /// <summary>
    /// Delete all information stored on the server
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MetadataCookiesFilter(delete: true)]
    [HttpDelete]
    public async Task Delete(CancellationToken cancellationToken)
    {
        await _questionService.Delete(Metadata, cancellationToken);
    }
}
