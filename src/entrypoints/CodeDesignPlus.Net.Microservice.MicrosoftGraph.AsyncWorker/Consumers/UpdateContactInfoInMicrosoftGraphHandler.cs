using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateContactInfo;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updatecontactinfo")]
public class UpdateContactInfoInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<UpdateContactInfoInMicrosoftGraphHandler> logger) : IEventHandler<ContactInfoUpdatedDomainEvent>
{
    public async Task HandleAsync(ContactInfoUpdatedDomainEvent data, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<Domain.UserAggregate>(data.AggregateId, token);

        if (!exists)
        {
            logger.LogInformation("User {Id} not found locally. Skipping Graph operation.", data.AggregateId);
            return;
        }

        var command = new UpdateContactInfoCommand(data.AggregateId, data.Contact);

        await mediator.Send(command, token);
    }
}
