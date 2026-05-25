using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.DeleteUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "deleteuser")]
public class DeleteUserInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<DeleteUserInMicrosoftGraphHandler> logger) : IEventHandler<UserDeletedDomainEvent>
{
    public async Task HandleAsync(UserDeletedDomainEvent data, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<Domain.UserAggregate>(data.AggregateId, token);

        if (!exists)
        {
            logger.LogInformation("User {Id} not found locally. Skipping Graph operation.", data.AggregateId);
            return;
        }

        var command = new DeleteUserCommand(data.AggregateId);

        await mediator.Send(command, token);
    }
}
