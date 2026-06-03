using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.DeleteGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class DeleteGroupInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidEvent_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var roleRepositoryMock = new Mock<IRoleRepository>();
        var loggerMock = new Mock<ILogger<DeleteGroupInMicrosoftGraphHandler>>();

        var aggregateId = Guid.NewGuid();
        roleRepositoryMock.Setup(x => x.ExistsAsync<Domain.RoleAggregate>(aggregateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteGroupInMicrosoftGraphHandler(mediatorMock.Object, roleRepositoryMock.Object, loggerMock.Object);
        var domainEvent = new RoleDeletedDomainEvent(aggregateId, "Test Group", "This is a test group", true);
        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<DeleteGroupCommand>(cmd => cmd.Id == domainEvent.AggregateId), cancellationToken), Times.Once);
    }
}
