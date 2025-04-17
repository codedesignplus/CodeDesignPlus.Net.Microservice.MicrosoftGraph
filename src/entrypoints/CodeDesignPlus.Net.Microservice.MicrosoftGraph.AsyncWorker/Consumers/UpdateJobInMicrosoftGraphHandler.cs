using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateJob;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "updatejob")]
public class UpdateJobInMicrosoftGraphHandler(IMediator mediator) : IEventHandler<JobInfoUpdatedDomainEvent>
{
    public Task HandleAsync(JobInfoUpdatedDomainEvent data, CancellationToken token)
    {
        var command = new UpdateJobCommand(data.AggregateId, data.Job);

        return mediator.Send(command, token);
        
    }
}
