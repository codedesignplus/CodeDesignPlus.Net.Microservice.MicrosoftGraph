using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "removegrouptouser")]
public class RemoveGroupToUserInMicrosoftGraphHandler(IMediator mediator, ILogger<RemoveGroupToUserInMicrosoftGraphHandler> logger) : IEventHandler<RoleRemovedToUserDomainEvent>
{
    public async Task HandleAsync(RoleRemovedToUserDomainEvent data, CancellationToken token)
    {
        try
        {
            var command = new RemoveGroupToUserCommand(data.AggregateId, data.Role);

            await mediator.Send(command, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to process {EventName} for {AggregateId}. Likely already processed. Skipping.", nameof(RoleRemovedToUserDomainEvent), data.AggregateId);
        }
    }
}
