using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateContactInfo;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updatecontactinfo")]
public class UpdateContactInfoInMicrosoftGraphHandler(IMediator mediator, ILogger<UpdateContactInfoInMicrosoftGraphHandler> logger) : IEventHandler<ContactInfoUpdatedDomainEvent>
{
    public async Task HandleAsync(ContactInfoUpdatedDomainEvent data, CancellationToken token)
    {
        try
        {
            var command = new UpdateContactInfoCommand(data.AggregateId, data.Contact);

            await mediator.Send(command, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to process {EventName} for {AggregateId}. Likely already processed. Skipping.", nameof(ContactInfoUpdatedDomainEvent), data.AggregateId);
        }
    }
}
