using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.DeleteUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class DeleteUserInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidEvent_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var handler = new DeleteUserInMicrosoftGraphHandler(mediatorMock.Object);

        var domainEvent = new UserDeletedDomainEvent(Guid.NewGuid(), "Joe", "Doe", "jd@fake.com", "3107531241", "Joe Doe", true);

        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(cmd => cmd.Id == domainEvent.AggregateId), cancellationToken), Times.Once);
    }
}
