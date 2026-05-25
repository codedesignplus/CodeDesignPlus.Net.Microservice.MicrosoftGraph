using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateJob;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updatejob")]
public class UpdateJobInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<UpdateJobInMicrosoftGraphHandler> logger) : IEventHandler<JobInfoUpdatedDomainEvent>
{
    public async Task HandleAsync(JobInfoUpdatedDomainEvent data, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<Domain.UserAggregate>(data.AggregateId, token);

        if (!exists)
        {
            logger.LogInformation("User {Id} not found locally. Skipping Graph operation.", data.AggregateId);
            return;
        }

        var command = new UpdateJobCommand(data.AggregateId, data.Job);

        await mediator.Send(command, token);
    }
}
