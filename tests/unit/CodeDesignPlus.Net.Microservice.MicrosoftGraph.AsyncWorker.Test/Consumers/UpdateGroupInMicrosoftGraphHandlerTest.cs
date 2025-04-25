using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class UpdateGroupInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidData_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var handler = new UpdateGroupInMicrosoftGraphHandler(mediatorMock.Object);

        var domainEvent = new RoleUpdatedDomainEvent(Guid.NewGuid(), "Test Group", "This is a test group", true);

        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<UpdateGroupCommand>(cmd =>
            cmd.Id == domainEvent.AggregateId &&
            cmd.Name == domainEvent.Name &&
            cmd.Description == domainEvent.Description &&
            cmd.IsActive == domainEvent.IsActive
        ), cancellationToken), Times.Once);
    }
}
