using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.CreateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("Graph", "creategroup")]
public class CreateGroupInMicrosoftGraphHandler(IMediator mediator, IRoleRepository roleRepository, ILogger<CreateGroupInMicrosoftGraphHandler> logger) : IEventHandler<RoleCreatedDomainEvent>
{
    public async Task HandleAsync(RoleCreatedDomainEvent data, CancellationToken token)
    {
        var exists = await roleRepository.ExistsAsync<Domain.RoleAggregate>(data.AggregateId, token);

        if (exists)
        {
            logger.LogInformation("Role/Group {RoleId} already exists. Skipping.", data.AggregateId);
            return;
        }

        var command = new CreateGroupCommand(data.AggregateId, data.Name, data.Description, data.IsActive);

        await mediator.Send(command, token);
    }
}
