using FluentAssertions;
using NSubstitute;
using Qotd.Application.Models;
using Qotd.Infrastructure.AI.Models;
using Qotd.Infrastructure.ChatGpt;
using Qotd.Infrastructure.Services;

namespace Qotd.UnitTests.Infrastructure;

public class QuestionServiceTests
{
    private static string _mockAssistantId = "aaa";
    private static string _mockThreadId = "bbb";
    private static string _mockRunId = "ccc";
    private static string _mockQuestion = "ddd";

    private readonly QuestionService _service;
    private readonly IAIClient _mockAIClient;

    public QuestionServiceTests()
    {
        _mockAIClient = Substitute.For<IAIClient>();

        _service = new QuestionService(_mockAIClient);
    }

    private static void MockSetup(IAIClient mockAIClient)
    {
        var mockSetupResponse = new ResponseBase
        {
            ThreadId = _mockThreadId,
            AssistantId = _mockAssistantId
        };
        mockAIClient.SetupThread(Arg.Any<CancellationToken>())
            .Returns(mockSetupResponse);
        mockAIClient.SetupThread(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(mockSetupResponse);
    }

    private static void MockRequest(IAIClient mockAIClient)
    {
        mockAIClient.RequestQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(x => new ResponseBase
            {
                AssistantId = (string)x[0],
                ThreadId = (string)x[1],
                RunId = _mockRunId
            });
    }

    private static void MockFetch(IAIClient mockAIClient)
    {
        mockAIClient.FetchQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(x => new Response
            {
                AssistantId = _mockAssistantId,
                ThreadId = (string)x[0],               
                RunId = (string)x[1],
                Question = _mockQuestion
            });
    }

    private static void MockDelete(IAIClient mockAIClient)
    {
        mockAIClient.Delete(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    #region Setup

    [Fact]
    public async void GivenNullMetadata_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockSetup(_mockAIClient);

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
        MockSetup(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", null },
                    { "ThreadId", _mockThreadId }
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
        MockSetup(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", "" },
                    { "ThreadId", _mockThreadId }
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
        MockSetup(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", _mockAssistantId },
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
        MockSetup(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", _mockAssistantId },
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

    [Fact]
    public async void GivenFullMetadata_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockSetup(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", _mockAssistantId },
                    { "ThreadId", _mockThreadId }
            }
        };

        // Act
        var sut = await _service.Setup(m, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    #endregion

    #region Generate

    [Fact]
    public async void GivenNullMetadata_WhenGenerateQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockRequest(_mockAIClient);

        // Act/Assert
        Func<Task> sut = async () => { await _service.GenerateQuestion(null, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async void GivenNullAssistantId_WhenGenerateQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockRequest(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", null },
                    { "ThreadId", _mockThreadId }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.GenerateQuestion(m, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async void GivenEmptyAssistantId_WhenGenerateQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockRequest(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", "" },
                    { "ThreadId", _mockThreadId }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.GenerateQuestion(m, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async void GivenNullThreadId_WhenGenerateQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockRequest(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", _mockAssistantId },
                    { "ThreadId", null }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.GenerateQuestion(m, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async void GivenEmptyThreadId_WhenGenerateQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockRequest(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", _mockAssistantId },
                    { "ThreadId", "" }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.GenerateQuestion(m, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async void GivenFullMetadata_WhenGenerateQuestionCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockRequest(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", _mockAssistantId },
                    { "ThreadId", _mockThreadId }
            }
        };

        // Act
        var sut = await _service.GenerateQuestion(m, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.QuestionId.Should().Be(_mockRunId);
        sut.Metadata.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Metadata.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    #endregion

    #region Get

    [Fact]
    public async void GivenNullMetadata_WhenGetQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockFetch(_mockAIClient);

        // Act/Assert
        Func<Task> sut = async () => { await _service.GetQuestion(null, _mockRunId, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async void GivenNullThreadId_WhenGetQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockFetch(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                { "ThreadId", null }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.GetQuestion(m, _mockRunId, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async void GivenEmptyThreadId_WhenGetQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockFetch(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                { "ThreadId", "" }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.GetQuestion(m, _mockRunId, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async void GivenNullRunId_WhenGetQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockFetch(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                { "ThreadId", _mockThreadId }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.GetQuestion(m, null, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async void GivenEmptyRunId_WhenGetQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockFetch(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                { "ThreadId", _mockThreadId }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.GetQuestion(m, "", CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async void GivenFullMetadata_WhenGetQuestionCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockFetch(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                { "ThreadId", _mockThreadId }
            }
        };

        // Act
        var sut = await _service.GetQuestion(m, _mockRunId, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Question.Should().Be(_mockQuestion);
        sut.Metadata.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    #endregion

    #region Delete

    [Fact]
    public async void GivenNullMetadata_WhenDeleteCalled_ThenExceptionThrown()
    {
        // Arrange
        MockDelete(_mockAIClient);

        // Act/Assert
        Func<Task> sut = async () => { await _service.Delete(null, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async void GivenNullAssistantId_WhenDeleteCalled_ThenExceptionThrown()
    {
        // Arrange
        MockDelete(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", null },
                    { "ThreadId", _mockThreadId }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.Delete(m, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async void GivenEmptyAssistantId_WhenDeleteCalled_ThenExceptionThrown()
    {
        // Arrange
        MockDelete(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", "" },
                    { "ThreadId", _mockThreadId }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.Delete(m, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async void GivenNullThreadId_WhenDeleteCalled_ThenExceptionThrown()
    {
        // Arrange
        MockDelete(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", _mockAssistantId },
                    { "ThreadId", null }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.Delete(m, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async void GivenEmptyThreadId_WhenDeleteCalled_ThenExceptionThrown()
    {
        // Arrange
        MockDelete(_mockAIClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", _mockAssistantId },
                    { "ThreadId", "" }
            }
        };

        // Act/Assert
        Func<Task> sut = async () => { await _service.Delete(m, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentException>();
    }

    #endregion
}
