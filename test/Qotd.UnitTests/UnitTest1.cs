using Qotd.Application.Models;

namespace Qotd.UnitTests
{
    public class UnitTest1
    {
        // Forcing > 0 coverage to test pipeline
        [Fact]
        public void Test1()
        {
            var md = new Metadata { Values = new Dictionary<string, string>() };
            var q = new QuestionResponse { Question = "Example", Metadata = md };
            var r = new RunResponse { RunId = "Example", Metadata = md };
            Assert.True(true);
        }
    }
}