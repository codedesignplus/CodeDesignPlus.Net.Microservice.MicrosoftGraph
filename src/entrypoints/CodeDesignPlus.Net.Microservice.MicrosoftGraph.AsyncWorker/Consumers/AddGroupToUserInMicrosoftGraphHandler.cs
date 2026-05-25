using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "addgrouptouser")]
public class AddGroupToUserInMicrosoftGraphHandler(IMediator mediator, ILogger<AddGroupToUserInMicrosoftGraphHandler> logger) : IEventHandler<RoleAddedToUserDomainEvent>
{
    public async Task HandleAsync(RoleAddedToUserDomainEvent data, CancellationToken token)
    {
        try
        {
            var command = new AddGroupToUserCommand(data.AggregateId, data.Role);

            await mediator.Send(command, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to process {EventName} for {AggregateId}. Likely already processed. Skipping.", nameof(RoleAddedToUserDomainEvent), data.AggregateId);
        }
    }
}
