using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;

[EventKey<UserAggregate>(1, "ProfileUpdatedDomainEvent", "ms-users-rest")]
public class ProfileUpdatedDomainEvent : UserBaseDomainEvent
{
    public ContactInfo Contact { get; }
    public JobInfo Job { get; }

    public ProfileUpdatedDomainEvent(
        Guid aggregateId,
        string firstName,
        string lastName,
        string email,
        string phone,
        string? displayName,
        bool isActive,
        ContactInfo contact,
        JobInfo job,
        Guid? eventId = null,
        Instant? occurredAt = null,
        Dictionary<string, object>? metadata = null
) : base(aggregateId, eventId, occurredAt, metadata)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        DisplayName = displayName;
        IsActive = isActive;
        Contact = contact;
        Job = job;
    }

    public static ProfileUpdatedDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string? displayName, bool isActive, ContactInfo contact, JobInfo job)
    {
        return new ProfileUpdatedDomainEvent(aggregateId, firstName, lastName, email, phone, displayName, isActive, contact, job);
    }
}
