using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;

[EventKey<UserAggregate>(1, "ContactInfoUpdatedDomainEvent", "ms-users")]
public class ContactInfoUpdatedDomainEvent(
     Guid aggregateId,
     ContactInfo contact,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public ContactInfo Contact { get; } = contact;
    public static ContactInfoUpdatedDomainEvent Create(Guid aggregateId, ContactInfo contact)
    {
        return new ContactInfoUpdatedDomainEvent(aggregateId, contact);
    }
}