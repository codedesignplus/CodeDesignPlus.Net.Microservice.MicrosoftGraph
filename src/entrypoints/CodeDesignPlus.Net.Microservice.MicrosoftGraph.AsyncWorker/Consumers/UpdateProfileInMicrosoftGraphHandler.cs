using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updateprofile")]
public class UpdateProfileInMicrosoftGraphHandler(IMediator mediator, ILogger<UpdateProfileInMicrosoftGraphHandler> logger) : IEventHandler<ProfileUpdatedDomainEvent>
{
    public async Task HandleAsync(ProfileUpdatedDomainEvent data, CancellationToken token)
    {
        try
        {
            var command = new UpdateProfileCommand(data.AggregateId, data.FirstName, data.LastName, data.DisplayName, data.Email, data.Phone, data.Contact, data.Job, data.IsActive);

            await mediator.Send(command, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to process {EventName} for {AggregateId}. Likely already processed. Skipping.", nameof(ProfileUpdatedDomainEvent), data.AggregateId);
        }
    }
}
