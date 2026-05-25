using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.DeleteUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "deleteuser")]
public class DeleteUserInMicrosoftGraphHandler(IMediator mediator, ILogger<DeleteUserInMicrosoftGraphHandler> logger) : IEventHandler<UserDeletedDomainEvent>
{
    public async Task HandleAsync(UserDeletedDomainEvent data, CancellationToken token)
    {
        try
        {
            var command = new DeleteUserCommand(data.AggregateId);

            await mediator.Send(command, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to process {EventName} for {AggregateId}. Likely already processed. Skipping.", nameof(UserDeletedDomainEvent), data.AggregateId);
        }
    }
}
