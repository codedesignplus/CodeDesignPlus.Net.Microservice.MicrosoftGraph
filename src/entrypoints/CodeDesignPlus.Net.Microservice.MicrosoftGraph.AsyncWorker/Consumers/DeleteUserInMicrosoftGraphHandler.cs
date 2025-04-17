using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.DeleteUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "deleteuser")]
public class DeleteUserInMicrosoftGraphHandler(IMediator mediator) : IEventHandler<UserDeletedDomainEvent>
{
    public Task HandleAsync(UserDeletedDomainEvent data, CancellationToken token)
    {
        var command = new DeleteUserCommand(data.AggregateId);

        return mediator.Send(command, token);    
    }
}
