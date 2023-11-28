using Microsoft.AspNetCore.Mvc;
using Qotd.Application.Interfaces;

namespace Qotd.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;
    private readonly ILogger<QuestionController> _logger;

    public QuestionController(IQuestionService questionService, ILogger<QuestionController> logger)
    {
        _questionService = questionService;
        _logger = logger;
    }

    [HttpGet(Name = "GetQuestion")]
    public async Task<string> Get()
    {
        Thread.Sleep(3000);

        return await _questionService.GetQuestionAsync();
    }
}
