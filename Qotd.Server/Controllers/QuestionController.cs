using Microsoft.AspNetCore.Mvc;
using Qotd.Application.Interfaces;

namespace Qotd.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet(Name = "GetQuestion")]
    public async Task<string> Get(CancellationToken cancellationToken)
    {
        return await _questionService.GetQuestion(cancellationToken);
    }
}
