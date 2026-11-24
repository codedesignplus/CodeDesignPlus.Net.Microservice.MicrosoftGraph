using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.SetAccountEnabled;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

/// <summary>
/// La cuenta que ms-users desactiva deja de poder entrar en Entra, y vuelve a poder cuando la reactiva.
/// </summary>
public class AccountStateInMicrosoftGraphHandlersTest
{
    private readonly Mock<IMediator> mediator = new();
    private readonly Mock<IUserRepository> repository = new();

    [Fact]
    public async Task HandleAsync_UserDeactivated_DisablesTheAccount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        repository.Setup(x => x.ExistsAsync<Domain.UserAggregate>(userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var handler = new DisableAccountInMicrosoftGraphHandler(mediator.Object, repository.Object, Mock.Of<ILogger<DisableAccountInMicrosoftGraphHandler>>());

        // Act
        await handler.HandleAsync(new UserDeactivatedDomainEvent(userId, "Laura Gómez"), CancellationToken.None);

        // Assert
        mediator.Verify(m => m.Send(It.Is<SetAccountEnabledCommand>(c => c.Id == userId && !c.Enabled), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserReactivated_EnablesTheAccount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        repository.Setup(x => x.ExistsAsync<Domain.UserAggregate>(userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var handler = new EnableAccountInMicrosoftGraphHandler(mediator.Object, repository.Object, Mock.Of<ILogger<EnableAccountInMicrosoftGraphHandler>>());

        // Act
        await handler.HandleAsync(new UserReactivatedDomainEvent(userId, "Laura Gómez"), CancellationToken.None);

        // Assert
        mediator.Verify(m => m.Send(It.Is<SetAccountEnabledCommand>(c => c.Id == userId && c.Enabled), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserNotReplicatedHere_SkipsGraph()
    {
        // Arrange
        var handler = new DisableAccountInMicrosoftGraphHandler(mediator.Object, repository.Object, Mock.Of<ILogger<DisableAccountInMicrosoftGraphHandler>>());

        // Act
        await handler.HandleAsync(new UserDeactivatedDomainEvent(Guid.NewGuid(), "Laura Gómez"), CancellationToken.None);

        // Assert
        mediator.Verify(m => m.Send(It.IsAny<SetAccountEnabledCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
