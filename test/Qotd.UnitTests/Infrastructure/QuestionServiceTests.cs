using FluentAssertions;
using NSubstitute;
using Qotd.Application.Interfaces;
using Qotd.Application.Models;
using Qotd.Infrastructure.AI.Models;
using Qotd.Infrastructure.ChatGpt;
using Qotd.Infrastructure.Services;

namespace Qotd.UnitTests.Infrastructure;

public class QuestionServiceTests
{
    private static string _mockAssistantId = "aaa";
    private static string _mockThreadId = "bbb";

    private readonly IQuestionService _service;

    public QuestionServiceTests()
    {
        var mockAIClient = Substitute.For<IAIClient>();
        mockAIClient.SetupThread(Arg.Any<CancellationToken>()).Returns(new ResponseBase { ThreadId = _mockThreadId, AssistantId = _mockAssistantId });
        mockAIClient.SetupThread(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(new ResponseBase { ThreadId = _mockThreadId, AssistantId = _mockAssistantId });

        _service = new QuestionService(mockAIClient);
    }

    [Fact]
    public async void GivenNullMetadata_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Act
        var sut = await _service.Setup(null, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    [Fact]
    public async void GivenNullAssistantId_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", null },
                    { "ThreadId", "imathread" }
            }
        };

        // Act
        var sut = await _service.Setup(m, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    [Fact]
    public async void GivenEmptyAssistantId_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", "" },
                    { "ThreadId", "imathread" }
            }
        };


        // Act
        var sut = await _service.Setup(m, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    [Fact]
    public async void GivenNullThreadId_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", "imanassistant" },
                    { "ThreadId", null }
            }
        };

        // Act
        var sut = await _service.Setup(m, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    [Fact]
    public async void GivenEmptyThreadId_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", "imanassistant" },
                    { "ThreadId", "" }
            }
        };

        // Act
        var sut = await _service.Setup(m, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

}
