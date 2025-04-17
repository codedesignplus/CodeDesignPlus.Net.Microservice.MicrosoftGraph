using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.DeleteGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("Graph", "deletegroup")]
public class DeleteGroupInMicrosoftGraphHandler(IMediator mediator) : IEventHandler<RoleDeletedDomainEvent>
{
    public Task HandleAsync(RoleDeletedDomainEvent data, CancellationToken token)
    {
        var command = new DeleteGroupCommand(data.AggregateId);
        
        mediator.Send(command, token);

        return Task.CompletedTask;
    }
}
