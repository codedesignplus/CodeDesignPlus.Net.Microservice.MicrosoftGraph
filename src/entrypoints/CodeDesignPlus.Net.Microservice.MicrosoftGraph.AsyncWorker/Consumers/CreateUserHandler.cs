using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "createuser")]
public class CreateUserHandler(IMediator mediator) : IEventHandler<UserCreatedDomainEvent>
{
    public Task HandleAsync(UserCreatedDomainEvent data, CancellationToken token)
    {
        var command = new CreateUserCommand(data.AggregateId, data.FirstName, data.LastName, data.Email, data.Phone, data.DisplayName, data.IsActive);

        return mediator.Send(command, token);
    }
}
