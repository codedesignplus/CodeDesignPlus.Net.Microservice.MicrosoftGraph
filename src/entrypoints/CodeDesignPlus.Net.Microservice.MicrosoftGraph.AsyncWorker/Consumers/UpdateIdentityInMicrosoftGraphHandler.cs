using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updateidentity")]
public class UpdateIdentityInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<UpdateIdentityInMicrosoftGraphHandler> logger) : IEventHandler<UserUpdatedDomainEvent>
{
    public async Task HandleAsync(UserUpdatedDomainEvent data, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<Domain.UserAggregate>(data.AggregateId, token);

        if (!exists)
        {
            logger.LogInformation("User {Id} not found locally. Skipping Graph operation.", data.AggregateId);
            return;
        }

        var command = new UpdateIdentityCommand(data.AggregateId, data.FirstName, data.LastName, data.DisplayName, data.Email, data.Phone, data.DocumentNumber, data.IsActive);

        await mediator.Send(command, token);
    }
}
