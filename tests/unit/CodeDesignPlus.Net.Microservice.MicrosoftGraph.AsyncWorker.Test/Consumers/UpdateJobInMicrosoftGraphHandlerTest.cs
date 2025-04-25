using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateJob;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class UpdateJobInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidData_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var handler = new UpdateJobInMicrosoftGraphHandler(mediatorMock.Object);

        var jobInfo = Domain.ValueObjects.JobInfo.Create("Software Engineer", "Tech Company", "It", "123456", "Full Time", SystemClock.Instance.GetCurrentInstant(), "Remote");


        var domainEvent = new JobInfoUpdatedDomainEvent(Guid.NewGuid(), jobInfo);

        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<UpdateJobCommand>(cmd =>
            cmd.Id == domainEvent.AggregateId &&
            cmd.Job == domainEvent.Job),
            cancellationToken), Times.Once);
    }
}
