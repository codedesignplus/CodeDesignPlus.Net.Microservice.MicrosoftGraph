namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;

[EventKey<UserAggregate>(1, "UserUpdatedDomainEvent", "ms-users")]
public class UserUpdatedDomainEvent : UserBaseDomainEvent
{
    public UserUpdatedDomainEvent(
        Guid aggregateId,
        string firstName,
        string lastName,
        string email,
        string phone,
        string? displayName,
        string documentNumber,
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
        DocumentNumber = documentNumber;
        IsActive = isActive;
    }

    public static UserUpdatedDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string? displayName, string documentNumber, bool isActive)
    {
        return new UserUpdatedDomainEvent(aggregateId, firstName, lastName, email, phone, displayName, documentNumber, isActive);
    }
}
