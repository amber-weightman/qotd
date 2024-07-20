using FluentAssertions;
using Qotd.Infrastructure.Services;

namespace Qotd.UnitTests.Infrastructure;

public class ApiKeyServiceTests
{
    private readonly IApiKeyService _service;

    public ApiKeyServiceTests()
    {
        _service = new ApiKeyService();
    }

    [Fact]
    public void GivenGenerateCalled_WhenGenerated_ThenHasExpectedLength()
    {
        // Act
        var sut = _service.Generate();

        // Assert
        sut.Should().HaveLength(32);
    }

    [Fact]
    public void GivenGenerateCalled_WhenGenerated_ThenHasExpectedPrefix()
    {
        // Act
        var sut = _service.Generate();

        // Assert
        sut.Should().StartWith("QOTD-");
    }

    [Fact]
    public void GivenGenerateCalled_WhenGenerated_ThenIsUriSafe()
    {
        // Act
        var sut = _service.Generate();

        // Assert
        Uri.IsWellFormedUriString(sut, UriKind.Relative);
    }
}
