using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "addgrouptouser")]
public class AddGroupToUserInMicrosoftGraphHandler(IMediator mediator) : IEventHandler<RoleAddedToUserDomainEvent>
{
    public Task HandleAsync(RoleAddedToUserDomainEvent data, CancellationToken token)
    {
        var command = new AddGroupToUserCommand(data.AggregateId, data.Role);

        return mediator.Send(command, token);
    }
}
