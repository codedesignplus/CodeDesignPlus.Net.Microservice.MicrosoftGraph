using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateJob;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updatejob")]
public class UpdateJobInMicrosoftGraphHandler(IMediator mediator, ILogger<UpdateJobInMicrosoftGraphHandler> logger) : IEventHandler<JobInfoUpdatedDomainEvent>
{
    public async Task HandleAsync(JobInfoUpdatedDomainEvent data, CancellationToken token)
    {
        try
        {
            var command = new UpdateJobCommand(data.AggregateId, data.Job);

            await mediator.Send(command, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to process {EventName} for {AggregateId}. Likely already processed. Skipping.", nameof(JobInfoUpdatedDomainEvent), data.AggregateId);
        }
    }
}
