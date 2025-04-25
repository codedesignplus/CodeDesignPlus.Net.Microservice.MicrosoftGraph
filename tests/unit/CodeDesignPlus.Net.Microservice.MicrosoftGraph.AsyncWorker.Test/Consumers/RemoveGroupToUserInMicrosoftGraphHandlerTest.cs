
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.User;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class RemoveGroupToUserInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidEvent_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var handler = new RemoveGroupToUserInMicrosoftGraphHandler(mediatorMock.Object);

        var domainEvent = new RoleRemovedToUserDomainEvent(Guid.NewGuid(), "Joe Doe", "Admin");

        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<RemoveGroupToUserCommand>(cmd =>
            cmd.Id == domainEvent.AggregateId &&
            cmd.Role == domainEvent.Role), cancellationToken), Times.Once);
    }
}
