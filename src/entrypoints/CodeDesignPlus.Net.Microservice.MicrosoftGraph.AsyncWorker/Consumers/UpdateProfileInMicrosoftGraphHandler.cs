using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updateprofile")]
public class UpdateProfileInMicrosoftGraphHandler(IMediator mediator) : IEventHandler<ProfileUpdatedDomainEvent>
{
    public Task HandleAsync(ProfileUpdatedDomainEvent data, CancellationToken token)
    {
        var command = new UpdateProfileCommand(data.AggregateId, data.FirtName, data.LastName, data.DisplayName, data.Email, data.Phone, data.Contact, data.Job, data.IsActive);

        return mediator.Send(command, token);
    }
}
