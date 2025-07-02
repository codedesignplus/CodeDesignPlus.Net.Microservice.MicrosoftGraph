namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;

[EventKey<UserAggregate>(1, "UserDeletedDomainEvent", "ms-users")]
public class UserDeletedDomainEvent : UserBaseDomainEvent
{
    public UserDeletedDomainEvent(
        Guid aggregateId,
        string firstName,
        string lastName,
        string email,
        string phone,
        string? displayName,
        bool isActive,
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
    }

    public static UserDeletedDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string? displayName, bool isActive)
    {
        return new UserDeletedDomainEvent(aggregateId, firstName, lastName, email, phone, displayName, isActive);
    }
}
