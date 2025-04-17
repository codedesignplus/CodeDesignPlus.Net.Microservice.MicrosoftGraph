using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateContactInfo;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updatecontactinfo")]
public class UpdateContactInfoInMicrosoftGraphHandler(IMediator mediator) : IEventHandler<ContactInfoUpdatedDomainEvent>
{
    public Task HandleAsync(ContactInfoUpdatedDomainEvent data, CancellationToken token)
    {
        var command = new UpdateContactInfoCommand(data.AggregateId, data.Contact);

        return mediator.Send(command, token);
    }
}
