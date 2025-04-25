using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateContactInfo;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class UpdateContactInfoInMicrosoftGraphHandlerTest
{
    [Fact]
    public async Task HandleAsync_ValidDomainEvent_CallsMediatorSend()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var handler = new UpdateContactInfoInMicrosoftGraphHandler(mediatorMock.Object);

        var contactInfo = Domain.ValueObjects.ContactInfo.Create("Street 123", "City", "State", "12345", "Country", "3105631234", ["joee.doe@fake.com"]);

        var domainEvent = new ContactInfoUpdatedDomainEvent(Guid.NewGuid(), contactInfo);

        var cancellationToken = CancellationToken.None;

        // Act
        await handler.HandleAsync(domainEvent, cancellationToken);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<UpdateContactInfoCommand>(cmd => cmd.Id == domainEvent.AggregateId && cmd.Contact == domainEvent.Contact), cancellationToken), Times.Once);
    }
}
