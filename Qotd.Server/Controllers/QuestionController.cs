using Microsoft.AspNetCore.Mvc;

namespace Qotd.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(ILogger<QuestionController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetQuestion")]
        public string Get()
        {
            Thread.Sleep(3000);

            return "What's the answer to life, the universe and everything? " + Guid.NewGuid();
        }
    }
}
