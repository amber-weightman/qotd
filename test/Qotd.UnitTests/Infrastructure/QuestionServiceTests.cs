using FluentAssertions;
using NSubstitute;
using Qotd.Application.Interfaces;
using Qotd.Application.Models;
using Qotd.Infrastructure.AI.Models;
using Qotd.Infrastructure.ChatGpt;
using Qotd.Infrastructure.Clients;
using Qotd.Infrastructure.Models;
using Qotd.Infrastructure.Services;

namespace Qotd.UnitTests.Infrastructure;

public class QuestionServiceTests
{
    private static string _mockAssistantId = "aaa";
    private static string _mockThreadId = "bbb";
    private static string _mockRunId = "ccc";
    private static string _mockQuestion = "ddd";

    private readonly IQuestionService _service;
    private readonly IAiClient _mockAiClient;
    private readonly IIpApiClient _mockIpClient;

    public QuestionServiceTests()
    {
        _mockAiClient = Substitute.For<IAiClient>();
        _mockIpClient = Substitute.For<IIpApiClient>();

        _service = new QuestionService(_mockAiClient, _mockIpClient);
    }

    private static void MockSetup(IAiClient mockAIClient)
    {
        var mockSetupResponse = new ResponseBase
        {
            ThreadId = _mockThreadId,
            AssistantId = _mockAssistantId
        };
        mockAIClient.SetupThread(Arg.Any<CancellationToken>())
            .Returns(mockSetupResponse);
        mockAIClient.SetupThread(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<Geolocation?>(), Arg.Any<CancellationToken>())
            .Returns(mockSetupResponse);
    }

    private static void MockRequest(IAiClient mockAIClient)
    {
        mockAIClient.RequestQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(x => new ResponseBase
            {
                AssistantId = (string)x[0],
                ThreadId = (string)x[1],
                RunId = _mockRunId
            });
    }

    private static void MockFetch(IAiClient mockAIClient)
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

    private static void MockDelete(IAiClient mockAIClient)
    {
        mockAIClient.Delete(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    #region Setup

    [Fact]
    public async Task GivenNullMetadata_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockSetup(_mockAiClient);

        // Act
        var sut = await _service.Setup(null, null, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    [Fact]
    public async Task GivenNullAssistantId_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockSetup(_mockAiClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", null },
                    { "ThreadId", _mockThreadId }
            }
        };

        // Act
        var sut = await _service.Setup(m, null, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    [Fact]
    public async Task GivenEmptyAssistantId_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockSetup(_mockAiClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", "" },
                    { "ThreadId", _mockThreadId }
            }
        };


        // Act
        var sut = await _service.Setup(m, null, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    [Fact]
    public async Task GivenNullThreadId_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockSetup(_mockAiClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", _mockAssistantId },
                    { "ThreadId", null }
            }
        };

        // Act
        var sut = await _service.Setup(m, null, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    [Fact]
    public async Task GivenEmptyThreadId_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockSetup(_mockAiClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", _mockAssistantId },
                    { "ThreadId", "" }
            }
        };

        // Act
        var sut = await _service.Setup(m, null, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    [Fact]
    public async Task GivenFullMetadata_WhenSetupCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockSetup(_mockAiClient);
        var m = new Metadata
        {
            Values = new Dictionary<string, string>
            {
                    { "AssistantId", _mockAssistantId },
                    { "ThreadId", _mockThreadId }
            }
        };

        // Act
        var sut = await _service.Setup(m, null, CancellationToken.None);

        // Assert
        sut.Should().NotBeNull();
        sut.Values.Should().Contain(new KeyValuePair<string, string>("AssistantId", _mockAssistantId));
        sut.Values.Should().Contain(new KeyValuePair<string, string>("ThreadId", _mockThreadId));
    }

    #endregion

    #region Generate

    [Fact]
    public async Task GivenNullMetadata_WhenGenerateQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockRequest(_mockAiClient);

        // Act/Assert
        Func<Task> sut = async () => { await _service.GenerateQuestion(null, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GivenNullAssistantId_WhenGenerateQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockRequest(_mockAiClient);
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
    public async Task GivenEmptyAssistantId_WhenGenerateQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockRequest(_mockAiClient);
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
    public async Task GivenNullThreadId_WhenGenerateQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockRequest(_mockAiClient);
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
    public async Task GivenEmptyThreadId_WhenGenerateQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockRequest(_mockAiClient);
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
    public async Task GivenFullMetadata_WhenGenerateQuestionCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockRequest(_mockAiClient);
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
    public async Task GivenNullMetadata_WhenGetQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockFetch(_mockAiClient);

        // Act/Assert
        Func<Task> sut = async () => { await _service.GetQuestion(null, _mockRunId, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GivenNullThreadId_WhenGetQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockFetch(_mockAiClient);
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
    public async Task GivenEmptyThreadId_WhenGetQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockFetch(_mockAiClient);
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
    public async Task GivenNullRunId_WhenGetQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockFetch(_mockAiClient);
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
    public async Task GivenEmptyRunId_WhenGetQuestionCalled_ThenExceptionThrown()
    {
        // Arrange
        MockFetch(_mockAiClient);
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
    public async Task GivenFullMetadata_WhenGetQuestionCalled_ThenValidMetadataReturned()
    {
        // Arrange
        MockFetch(_mockAiClient);
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
    public async Task GivenNullMetadata_WhenDeleteCalled_ThenExceptionThrown()
    {
        // Arrange
        MockDelete(_mockAiClient);

        // Act/Assert
        Func<Task> sut = async () => { await _service.Delete(null, CancellationToken.None); };
        await sut.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async void GivenNullAssistantId_WhenDeleteCalled_ThenExceptionThrown()
    {
        // Arrange
        MockDelete(_mockAiClient);
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
    public async Task GivenEmptyAssistantId_WhenDeleteCalled_ThenExceptionThrown()
    {
        // Arrange
        MockDelete(_mockAiClient);
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
    public async Task GivenNullThreadId_WhenDeleteCalled_ThenExceptionThrown()
    {
        // Arrange
        MockDelete(_mockAiClient);
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
    public async Task GivenEmptyThreadId_WhenDeleteCalled_ThenExceptionThrown()
    {
        // Arrange
        MockDelete(_mockAiClient);
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
