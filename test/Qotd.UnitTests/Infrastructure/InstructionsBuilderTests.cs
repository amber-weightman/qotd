using FluentAssertions;
using Qotd.Infrastructure.AI;

namespace Qotd.UnitTests.Infrastructure;

public class InstructionsBuilderTests
{
    [Fact]
    public void TestInstructions()
    {
        var sut = InstructionsBuilder.GetInstructions();
        sut.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void TestPrompt()
    {
        var sut = InstructionsBuilder.GetPrompt();
        sut.Should().NotBeNullOrEmpty();
    }
}
