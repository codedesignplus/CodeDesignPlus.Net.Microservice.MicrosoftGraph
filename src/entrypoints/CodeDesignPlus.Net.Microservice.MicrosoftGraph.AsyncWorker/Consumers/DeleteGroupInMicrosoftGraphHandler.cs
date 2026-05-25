using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.DeleteGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("Graph", "deletegroup")]
public class DeleteGroupInMicrosoftGraphHandler(IMediator mediator, ILogger<DeleteGroupInMicrosoftGraphHandler> logger) : IEventHandler<RoleDeletedDomainEvent>
{
    public async Task HandleAsync(RoleDeletedDomainEvent data, CancellationToken token)
    {
        try
        {
            var command = new DeleteGroupCommand(data.AggregateId);

            await mediator.Send(command, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to process {EventName} for {AggregateId}. Likely already processed. Skipping.", nameof(RoleDeletedDomainEvent), data.AggregateId);
        }
    }
}
