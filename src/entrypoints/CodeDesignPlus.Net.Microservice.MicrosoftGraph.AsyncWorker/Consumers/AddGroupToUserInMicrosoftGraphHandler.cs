using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "addgrouptouser")]
public class AddGroupToUserInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<AddGroupToUserInMicrosoftGraphHandler> logger) : IEventHandler<RoleAddedToUserDomainEvent>
{
    public async Task HandleAsync(RoleAddedToUserDomainEvent data, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<Domain.UserAggregate>(data.AggregateId, token);

        if (!exists)
        {
            logger.LogInformation("User {Id} not found locally. Skipping Graph operation.", data.AggregateId);
            return;
        }

        var command = new AddGroupToUserCommand(data.AggregateId, data.Role);

        await mediator.Send(command, token);
    }
}
