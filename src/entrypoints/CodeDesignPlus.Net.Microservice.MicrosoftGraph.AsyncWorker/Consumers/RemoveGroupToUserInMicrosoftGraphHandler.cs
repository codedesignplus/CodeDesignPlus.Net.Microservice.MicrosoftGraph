using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "removegrouptouser")]
public class RemoveGroupToUserInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<RemoveGroupToUserInMicrosoftGraphHandler> logger) : IEventHandler<RoleRemovedToUserDomainEvent>
{
    public async Task HandleAsync(RoleRemovedToUserDomainEvent data, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<Domain.UserAggregate>(data.AggregateId, token);

        if (!exists)
        {
            logger.LogInformation("User {Id} not found locally. Skipping Graph operation.", data.AggregateId);
            return;
        }

        var command = new RemoveGroupToUserCommand(data.AggregateId, data.Role);

        await mediator.Send(command, token);
    }
}
