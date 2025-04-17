using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.User;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "removegrouptouser")]
public class RemoveGroupToUserInMicrosoftGraphHandler(IMediator mediator) : IEventHandler<RoleRemovedToUserDomainEvent>
{
    public Task HandleAsync(RoleRemovedToUserDomainEvent data, CancellationToken token)
    {
        var command = new RemoveGroupToUserCommand(data.AggregateId, data.Role);

        return mediator.Send(command, token);
    }
}
