namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;

[EventKey<RoleAggregate>(1, "RoleDeletedDomainEvent", "ms-roles-rest")]
public class RoleDeletedDomainEvent(
    Guid aggregateId,
    string name,
    string description,
    bool isActive,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{

    public string Name { get; private set; } = name;

    public string Description { get; private set; } = description;

    public bool IsActive { get; private set; } = isActive;

    public static RoleDeletedDomainEvent Create(Guid aggregateId, string name, string description, bool isActive)
    {
        return new RoleDeletedDomainEvent(aggregateId, name, description, isActive);
    }
}
