using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class AddGroupToUserInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidEvent_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var handler = new AddGroupToUserInMicrosoftGraphHandler(mediatorMock.Object);

        var domainEvent = new RoleAddedToUserDomainEvent(Guid.NewGuid(), "Joe Doe", "Admin");

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
}
