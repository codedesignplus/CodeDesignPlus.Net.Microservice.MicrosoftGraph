using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class UpdateProfileInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidData_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var handler = new UpdateProfileInMicrosoftGraphHandler(mediatorMock.Object);

        var contactInfo = Domain.ValueObjects.ContactInfo.Create("Street 123", "City", "State", "12345", "Country", "3105631234", ["joee.doe@fake.com"]);
        var jobInfo = Domain.ValueObjects.JobInfo.Create("Software Engineer", "Tech Company", "It", "123456", "Full Time", SystemClock.Instance.GetCurrentInstant(), "Remote");

        var domainEvent = new ProfileUpdatedDomainEvent(Guid.NewGuid(), "Joe", "Doe", "jd@fake.com", "3107531241", "Joe Doe", true, contactInfo, jobInfo);

        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<UpdateProfileCommand>(command =>
            command.Id == domainEvent.AggregateId &&
            command.FirstName == domainEvent.FirstName &&
            command.LastName == domainEvent.LastName &&
            command.DisplayName == domainEvent.DisplayName &&
            command.Email == domainEvent.Email &&
            command.Phone == domainEvent.Phone &&
            command.Contact == domainEvent.Contact &&
            command.Job == domainEvent.Job &&
            command.IsActive == domainEvent.IsActive
        ), cancellationToken), Times.Once);
    }
}
