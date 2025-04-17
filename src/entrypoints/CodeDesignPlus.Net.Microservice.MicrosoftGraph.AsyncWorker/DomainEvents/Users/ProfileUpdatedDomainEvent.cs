using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;

[EventKey<UserAggregate>(1, "ProfileUpdatedDomainEvent", "ms-users")]
public class ProfileUpdatedDomainEvent(
     Guid aggregateId,
     ContactInfo contact,
     JobInfo job,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) :  UserBaseDomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public ContactInfo Contact { get; } = contact;
    public JobInfo Job { get; } = job;

    public static ProfileUpdatedDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string? displayName, bool isActive, ContactInfo contact, JobInfo job)
    {
        return new ProfileUpdatedDomainEvent(aggregateId, contact, job)
        {
            FirtName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            DisplayName = displayName,
            IsActive = isActive,
        };
    }
}
