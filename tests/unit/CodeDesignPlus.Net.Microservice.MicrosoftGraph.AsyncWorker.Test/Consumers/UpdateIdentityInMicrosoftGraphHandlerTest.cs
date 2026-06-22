using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class UpdateIdentityInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidEvent_CallsMediatorSendWithCorrectCommand()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var loggerMock = new Mock<ILogger<UpdateIdentityInMicrosoftGraphHandler>>();

        var aggregateId = Guid.NewGuid();
        userRepositoryMock.Setup(x => x.ExistsAsync<Domain.UserAggregate>(aggregateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateIdentityInMicrosoftGraphHandler(mediatorMock.Object, userRepositoryMock.Object, loggerMock.Object);

        var domainEvent = new UserUpdatedDomainEvent(aggregateId, "Joe", "Doe", "jd@fake.com", "3107531241", "Joe Doe", "1234567890", true);

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
