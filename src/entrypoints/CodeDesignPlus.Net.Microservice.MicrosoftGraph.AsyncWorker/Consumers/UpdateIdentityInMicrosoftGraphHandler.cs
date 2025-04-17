using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updateidentity")]
public class UpdateIdentityInMicrosoftGraphHandler(IMediator mediator) : IEventHandler<UserUpdatedDomainEvent>
{
    public Task HandleAsync(UserUpdatedDomainEvent data, CancellationToken token)
    {
        var command = new UpdateIdentityCommand(data.AggregateId, data.FirtName, data.LastName, data.DisplayName, data.Email, data.Phone, data.IsActive);

        return mediator.Send(command, token);
    }
}
