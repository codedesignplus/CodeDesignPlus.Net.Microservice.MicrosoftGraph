using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updateprofile")]
public class UpdateProfileInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<UpdateProfileInMicrosoftGraphHandler> logger) : IEventHandler<ProfileUpdatedDomainEvent>
{
    public async Task HandleAsync(ProfileUpdatedDomainEvent data, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<Domain.UserAggregate>(data.AggregateId, token);

        if (!exists)
        {
            logger.LogInformation("User {Id} not found locally. Skipping Graph operation.", data.AggregateId);
            return;
        }

        var command = new UpdateProfileCommand(data.AggregateId, data.FirstName, data.LastName, data.DisplayName, data.Email, data.Phone, data.Contact, data.Job, data.IsActive);

        await mediator.Send(command, token);
    }
}
