using System;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;

public abstract class UserBaseDomainEvent(
     Guid aggregateId,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string FirtName { get; protected set; } = null!;
    public string LastName { get; protected set; } = null!;
    public string Email { get; protected set; } = null!;
    public string Phone { get; protected set; } = null!;
    public string? DisplayName { get; protected set; } = null!;
    public bool IsActive { get; protected set; }
}
