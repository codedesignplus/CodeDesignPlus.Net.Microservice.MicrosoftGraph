namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;

/// <summary>
/// External event raised by ms-users when a new <c>UserAggregate</c> is registered.
/// Mirrors the canonical event published by ms-users (named "UserRegistered" — distinct
/// from this microservice's own <c>UserCreatedDomainEvent</c> for the Azure AD identity)
/// so the AsyncWorker can subscribe and forward the creation into Microsoft Graph
/// through the local CreateUserCommand.
///
/// Serialized key: <c>codedesignplus.ms-users.v1.useraggregate.userregistereddomainevent</c>.
/// </summary>
[EventKey("UserAggregate", 1, "UserRegisteredDomainEvent", "ms-users")]
public class UserRegisteredDomainEvent : UserBaseDomainEvent
{
    public UserRegisteredDomainEvent(
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
}
