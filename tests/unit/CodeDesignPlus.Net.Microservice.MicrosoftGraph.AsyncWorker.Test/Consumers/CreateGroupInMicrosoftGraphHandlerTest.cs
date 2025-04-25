using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.CreateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class CreateGroupInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidEvent_CallsMediatorSendWithCorrectCommand()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var handler = new CreateGroupInMicrosoftGraphHandler(mediatorMock.Object);

        var domainEvent = new RoleCreatedDomainEvent(
            Guid.NewGuid(),
            "Test Group",
            "This is a test group",
            true
        );

        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<CreateGroupCommand>(command =>
            command.Id == domainEvent.AggregateId &&
            command.Name == domainEvent.Name &&
            command.Description == domainEvent.Description &&
            command.IsActive == domainEvent.IsActive
        ), cancellationToken), Times.Once);
    }
}
