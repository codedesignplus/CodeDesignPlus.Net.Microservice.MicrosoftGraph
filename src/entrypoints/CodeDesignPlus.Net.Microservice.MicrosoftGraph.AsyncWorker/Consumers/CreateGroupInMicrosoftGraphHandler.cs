using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.CreateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("Graph", "creategroup")]
public class CreateGroupInMicrosoftGraphHandler(IMediator mediator) : IEventHandler<RoleCreatedDomainEvent>
{
    public Task HandleAsync(RoleCreatedDomainEvent data, CancellationToken token)
    {
        var command = new CreateGroupCommand(data.AggregateId, data.Name, data.Description, data.IsActive);

        mediator.Send(command, token);

        return Task.CompletedTask;
    }
}
