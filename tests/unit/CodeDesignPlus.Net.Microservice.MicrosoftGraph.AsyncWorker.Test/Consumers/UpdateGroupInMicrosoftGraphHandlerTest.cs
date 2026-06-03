using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class UpdateGroupInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidData_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var roleRepositoryMock = new Mock<IRoleRepository>();
        var loggerMock = new Mock<ILogger<UpdateGroupInMicrosoftGraphHandler>>();

        var aggregateId = Guid.NewGuid();
        roleRepositoryMock.Setup(x => x.ExistsAsync<Domain.RoleAggregate>(aggregateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateGroupInMicrosoftGraphHandler(mediatorMock.Object, roleRepositoryMock.Object, loggerMock.Object);

        var domainEvent = new RoleUpdatedDomainEvent(aggregateId, "Test Group", "This is a test group", true);

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
