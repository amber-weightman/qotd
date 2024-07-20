using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qotd.Api.Controllers;
using Qotd.Api.Extensions;
using Qotd.Api.Filters;
using Qotd.Application.Interfaces;
using Qotd.Application.Models;

namespace Qotd.Server.Controllers;

/// <summary>
/// A group of endpoints for generating questions
/// </summary>
[ApiController]
//[Authorize]
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
    /// 1. Initialise your personal AI assistant
    /// </summary>
    /// <remarks>
    /// AI assistant must be initialised first before questions can be retrieved
    /// </remarks>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns></returns>
    [MetadataCookiesFilter]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
    [HttpGet("setup")]
    public async Task Setup(CancellationToken cancellationToken)
    {
        var ip = HttpContext.GetIp();
        var response = await _questionService.Setup(Metadata!, ip, cancellationToken);
        Metadata = response;
    }

    /// <summary>
    /// 2. Ask your pesonal AI assistant to think of a question for you
    /// </summary>
    /// <remarks>
    /// Returns a text/plain `QuestionId` which can be used to retrieve the question
    /// </remarks>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A text/plain `QuestionId` which can be used to retrieve the question</returns>
    [MetadataCookiesFilter]
    [HttpGet("generate-question")]
    public async Task<string> GenerateQuestion(CancellationToken cancellationToken)
    {
        var response = await _questionService.GenerateQuestion(Metadata!, cancellationToken);

        Metadata = response.Metadata;

        return response.QuestionId;
    }

    /// <summary>
    /// 3. Retrieve the question
    /// </summary>
    /// <remarks>
    /// Returns a text/plain `Question`
    /// </remarks>
    /// <param name="questionId">`QuestionId` which can be used to retrieve the question</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Your very own personal Question of the Day</returns>
    [MetadataCookiesFilter]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 286400)]
    [HttpGet("get-question/{questionId}")]
    public async Task<string> GetQuestion([FromRoute]string questionId, CancellationToken cancellationToken)
    {
        var response = await _questionService.GetQuestion(Metadata!, questionId, cancellationToken);

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
        await _questionService.Delete(Metadata!, cancellationToken);
    }
}
