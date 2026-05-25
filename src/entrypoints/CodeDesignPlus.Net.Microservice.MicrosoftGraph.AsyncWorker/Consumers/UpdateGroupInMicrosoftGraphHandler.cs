using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("Graph", "updategroup")]
public class UpdateGroupInMicrosoftGraphHandler(IMediator mediator, IRoleRepository roleRepository, ILogger<UpdateGroupInMicrosoftGraphHandler> logger) : IEventHandler<RoleUpdatedDomainEvent>
{
    public async Task HandleAsync(RoleUpdatedDomainEvent data, CancellationToken token)
    {
        var exists = await roleRepository.ExistsAsync<Domain.RoleAggregate>(data.AggregateId, token);

        if (!exists)
        {
            logger.LogInformation("Role {Id} not found locally. Skipping Graph operation.", data.AggregateId);
            return;
        }

        var command = new UpdateGroupCommand(data.AggregateId, data.Name, data.Description, data.IsActive);

        await mediator.Send(command, token);
    }
}
