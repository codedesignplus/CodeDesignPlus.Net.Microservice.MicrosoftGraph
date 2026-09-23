
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class RemoveGroupToUserInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidEvent_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var loggerMock = new Mock<ILogger<RemoveGroupToUserInMicrosoftGraphHandler>>();

        var aggregateId = Guid.NewGuid();
        userRepositoryMock.Setup(x => x.ExistsAsync<Domain.UserAggregate>(aggregateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new RemoveGroupToUserInMicrosoftGraphHandler(mediatorMock.Object, userRepositoryMock.Object, loggerMock.Object);

        var domainEvent = new RoleRemovedToUserDomainEvent(aggregateId, "Joe Doe", Guid.NewGuid(), Guid.NewGuid(), stillHasItElsewhere: false);

        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<RemoveGroupToUserCommand>(cmd =>
            cmd.Id == domainEvent.AggregateId &&
            cmd.Role == domainEvent.Role), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_RoleSurvivesInAnotherTenant_LeavesTheGroupAlone()
    {
        // El grupo del proveedor de identidad es global. Sacar al usuario de el por haber perdido el rol en
        // una sola copropiedad se lo quitaria en todas las demas, y nada lo avisaria: el evento se habria
        // consumido sin error.
        var mediatorMock = new Mock<IMediator>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var loggerMock = new Mock<ILogger<RemoveGroupToUserInMicrosoftGraphHandler>>();

        var handler = new RemoveGroupToUserInMicrosoftGraphHandler(mediatorMock.Object, userRepositoryMock.Object, loggerMock.Object);

        var domainEvent = new RoleRemovedToUserDomainEvent(
            Guid.NewGuid(), "Joe Doe", Guid.NewGuid(), Guid.NewGuid(), stillHasItElsewhere: true);

        // Act
        await handler.HandleAsync(domainEvent, CancellationToken.None);

        // Assert
        mediatorMock.Verify(m => m.Send(It.IsAny<RemoveGroupToUserCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        userRepositoryMock.Verify(x => x.ExistsAsync<Domain.UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
