namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "UserCreatedDomainEvent")]
public class UserCreatedDomainEvent(
    Guid aggregateId,
    string firstName,
    string lastName,
    string email,
    string phone,
    string? displayName,
    string password,
    bool isActive,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public string Email { get; private set; } = email;
    public string Phone { get; private set; } = phone;
    public string? DisplayName { get; private set; } = displayName;
    public string Password { get; private set; } = password;
    public bool IsActive { get; private set; } = isActive;
    public static UserCreatedDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string? displayName, string password, bool isActive)
    {
        return new UserCreatedDomainEvent(aggregateId, firstName, lastName, email, phone, displayName, password, isActive);
    }
}
