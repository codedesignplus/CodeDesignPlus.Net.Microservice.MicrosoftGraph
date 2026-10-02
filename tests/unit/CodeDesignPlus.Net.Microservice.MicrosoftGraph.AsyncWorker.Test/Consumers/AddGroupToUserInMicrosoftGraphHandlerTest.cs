using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class AddGroupToUserInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidEvent_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var loggerMock = new Mock<ILogger<AddGroupToUserInMicrosoftGraphHandler>>();

        var aggregateId = Guid.NewGuid();
        userRepositoryMock.Setup(x => x.ExistsAsync<Domain.UserAggregate>(aggregateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new AddGroupToUserInMicrosoftGraphHandler(mediatorMock.Object, userRepositoryMock.Object, loggerMock.Object);

        var domainEvent = new RoleAddedToUserDomainEvent(aggregateId, "Joe Doe", Guid.NewGuid(), Guid.NewGuid());

        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<AddGroupToUserCommand>(cmd =>
            cmd.Id == domainEvent.AggregateId &&
            cmd.Role == domainEvent.Role), cancellationToken),
            Times.Once
        );
    }
    /// <summary>
    /// El rol llega antes de que termine el alta: no se descarta, se lanza para que el bus lo reintente.
    /// </summary>
    [Fact]
    public async Task HandleAsync_UserNotProvisionedYet_ThrowsSoTheBusRetries()
    {
        var mediatorMock = new Mock<IMediator>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var loggerMock = new Mock<ILogger<AddGroupToUserInMicrosoftGraphHandler>>();

        var aggregateId = Guid.NewGuid();
        userRepositoryMock.Setup(x => x.ExistsAsync<Domain.UserAggregate>(aggregateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new AddGroupToUserInMicrosoftGraphHandler(mediatorMock.Object, userRepositoryMock.Object, loggerMock.Object);
        var domainEvent = new RoleAddedToUserDomainEvent(aggregateId, "Joe Doe", Guid.NewGuid(), Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<UserNotProvisionedYetException>(() => handler.HandleAsync(domainEvent, CancellationToken.None));

        Assert.Equal(aggregateId, exception.UserId);
        mediatorMock.Verify(m => m.Send(It.IsAny<AddGroupToUserCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// El bus manda a dead-letter sin reintentar todo error de negocio: la excepcion no puede serlo.
    /// </summary>
    [Fact]
    public void UserNotProvisionedYetException_IsNotABusinessError()
    {
        Assert.False(typeof(CodeDesignPlus.Net.Exceptions.CodeDesignPlusException).IsAssignableFrom(typeof(UserNotProvisionedYetException)));
    }
}
