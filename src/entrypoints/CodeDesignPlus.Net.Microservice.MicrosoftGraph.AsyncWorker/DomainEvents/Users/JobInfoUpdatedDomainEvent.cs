using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;

[EventKey<UserAggregate>(1, "JobInfoUpdatedDomainEvent", "ms-users")]
public class JobInfoUpdatedDomainEvent(
    Guid aggregateId,
    JobInfo job,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public JobInfo Job { get; } = job;

    public static JobInfoUpdatedDomainEvent Create(Guid aggregateId, JobInfo job)
    {
        return new JobInfoUpdatedDomainEvent(aggregateId, job);
    }
}
