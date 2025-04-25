using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class UpdateIdentityInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidEvent_CallsMediatorSendWithCorrectCommand()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var handler = new UpdateIdentityInMicrosoftGraphHandler(mediatorMock.Object);

        var domainEvent = new UserUpdatedDomainEvent(Guid.NewGuid(), "Joe", "Doe", "jd@fake.com", "3107531241", "Joe Doe", true);

        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(mediator => mediator.Send(
            It.Is<UpdateIdentityCommand>(command =>
                command.Id == domainEvent.AggregateId &&
                command.FirstName == domainEvent.FirstName &&
                command.LastName == domainEvent.LastName &&
                command.DisplayName == domainEvent.DisplayName &&
                command.Email == domainEvent.Email &&
                command.Phone == domainEvent.Phone &&
                command.IsActive == domainEvent.IsActive),
            cancellationToken),
            Times.Once);
    }
}
