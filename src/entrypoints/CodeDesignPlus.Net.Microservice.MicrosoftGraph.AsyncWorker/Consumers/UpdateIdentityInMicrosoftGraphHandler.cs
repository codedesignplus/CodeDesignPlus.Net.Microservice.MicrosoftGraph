using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updateidentity")]
public class UpdateIdentityInMicrosoftGraphHandler(IMediator mediator, ILogger<UpdateIdentityInMicrosoftGraphHandler> logger) : IEventHandler<UserUpdatedDomainEvent>
{
    public async Task HandleAsync(UserUpdatedDomainEvent data, CancellationToken token)
    {
        try
        {
            var command = new UpdateIdentityCommand(data.AggregateId, data.FirstName, data.LastName, data.DisplayName, data.Email, data.Phone, data.IsActive);

            await mediator.Send(command, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to process {EventName} for {AggregateId}. Likely already processed. Skipping.", nameof(UserUpdatedDomainEvent), data.AggregateId);
        }
    }
}
