using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("Graph", "updategroup")]
public class UpdateGroupInMicrosoftGraphHandler(IMediator mediator) : IEventHandler<RoleUpdatedDomainEvent>
{
    public Task HandleAsync(RoleUpdatedDomainEvent data, CancellationToken token)
    {
        var command = new UpdateGroupCommand(data.AggregateId, data.Name, data.Description, data.IsActive);

        return mediator.Send(command, token);
    }
}
