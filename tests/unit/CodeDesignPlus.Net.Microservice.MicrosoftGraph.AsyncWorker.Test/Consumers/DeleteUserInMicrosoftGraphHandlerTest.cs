using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.DeleteUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class DeleteUserInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidEvent_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var loggerMock = new Mock<ILogger<DeleteUserInMicrosoftGraphHandler>>();

        var aggregateId = Guid.NewGuid();
        userRepositoryMock.Setup(x => x.ExistsAsync<Domain.UserAggregate>(aggregateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteUserInMicrosoftGraphHandler(mediatorMock.Object, userRepositoryMock.Object, loggerMock.Object);

        var domainEvent = new UserDeletedDomainEvent(aggregateId, "Joe", "Doe", "jd@fake.com", "3107531241", "Joe Doe", true);

        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(cmd => cmd.Id == domainEvent.AggregateId), cancellationToken), Times.Once);
    }
}
