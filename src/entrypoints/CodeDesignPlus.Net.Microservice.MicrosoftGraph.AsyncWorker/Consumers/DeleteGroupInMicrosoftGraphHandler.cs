using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.DeleteGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("Graph", "deletegroup")]
public class DeleteGroupInMicrosoftGraphHandler(IMediator mediator, IRoleRepository roleRepository, ILogger<DeleteGroupInMicrosoftGraphHandler> logger) : IEventHandler<RoleDeletedDomainEvent>
{
    public async Task HandleAsync(RoleDeletedDomainEvent data, CancellationToken token)
    {
        var exists = await roleRepository.ExistsAsync<Domain.RoleAggregate>(data.AggregateId, token);

        if (!exists)
        {
            logger.LogInformation("Role {Id} not found locally. Skipping Graph operation.", data.AggregateId);
            return;
        }

        var command = new DeleteGroupCommand(data.AggregateId);

        await mediator.Send(command, token);
    }
}
