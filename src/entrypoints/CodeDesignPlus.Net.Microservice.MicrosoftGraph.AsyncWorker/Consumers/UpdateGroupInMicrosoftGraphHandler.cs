using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("Graph", "updategroup")]
public class UpdateGroupInMicrosoftGraphHandler(IMediator mediator, ILogger<UpdateGroupInMicrosoftGraphHandler> logger) : IEventHandler<RoleUpdatedDomainEvent>
{
    public async Task HandleAsync(RoleUpdatedDomainEvent data, CancellationToken token)
    {
        try
        {
            var command = new UpdateGroupCommand(data.AggregateId, data.Name, data.Description, data.IsActive);

            await mediator.Send(command, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to process {EventName} for {AggregateId}. Likely already processed. Skipping.", nameof(RoleUpdatedDomainEvent), data.AggregateId);
        }
    }
}
