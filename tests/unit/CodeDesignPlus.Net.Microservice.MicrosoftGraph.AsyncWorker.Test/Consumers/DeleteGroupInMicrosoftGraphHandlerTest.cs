using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.DeleteGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class DeleteGroupInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidEvent_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var handler = new DeleteGroupInMicrosoftGraphHandler(mediatorMock.Object);
        var domainEvent = new RoleDeletedDomainEvent(Guid.NewGuid(), "Test Group", "This is a test group", true);
        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<DeleteGroupCommand>(cmd => cmd.Id == domainEvent.AggregateId), cancellationToken), Times.Once);
    }
}
