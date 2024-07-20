using FluentAssertions;
using Qotd.Application.Enums;
using Qotd.Application.Interfaces;
using Qotd.Application.Models;
using Qotd.Infrastructure.Services;

namespace Qotd.UnitTests.Infrastructure;

public class ClientServiceTests
{
    private readonly IClientService _service;

    private static readonly string _systemKey = "system_key";

    public ClientServiceTests()
    {
        _service = new ClientService(_systemKey);
    }

    [Fact]
    public void GivenAuthenticateClientCalled_WhenKeyMatches_ThenAuthenticates()
    {
        // Act
        var sut = _service.AuthenticateClient(_systemKey);

        // Assert
        sut.Should().BeEquivalentTo(new AuthenticationResponse
        {
            ClientName = "DEFAULT",
            ClientType = AuthenticationClientType.DefaultPublic,
            IsAuthenticated = true
        });
    }

    [Fact]
    public void GivenAuthenticateClientCalled_WhenKeyDoesNotMatch_ThenDoesNotAuthenticates()
    {
        // Act
        var sut = _service.AuthenticateClient("invalid_key");

        // Assert
        sut.Should().BeEquivalentTo(new AuthenticationResponse
        {
            ClientName = "ANONYMOUS",
            ClientType = AuthenticationClientType.Unknown,
            IsAuthenticated = false
        });
    }

}
