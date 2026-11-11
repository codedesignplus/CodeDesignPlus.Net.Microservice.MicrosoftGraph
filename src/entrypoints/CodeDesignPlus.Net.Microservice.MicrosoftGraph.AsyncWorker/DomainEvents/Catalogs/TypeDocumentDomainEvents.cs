namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Catalogs;

/// <summary>
/// Forma comun de los eventos de tipo de documento que publica ms-catalogs, que es su dueno (pendings/663).
/// </summary>
public abstract class TypeDocumentBaseDomainEvent(Guid aggregateId, string name, string? description, string code, bool isActive, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
    : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    /// <summary>
    /// Gets the display name of the type.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the description of the type.
    /// </summary>
    public string? Description { get; } = description;

    /// <summary>
    /// Gets the catalog code.
    /// </summary>
    public string Code { get; } = code;

    /// <summary>
    /// Gets whether the type can be chosen.
    /// </summary>
    public bool IsActive { get; } = isActive;
}

/// <summary>
/// Copia local del alta de un tipo de documento en ms-catalogs.
/// </summary>
[EventKey("TypeDocumentAggregate", 1, "TypeDocumentCreatedDomainEvent", "ms-catalogs")]
public class TypeDocumentCreatedDomainEvent(Guid aggregateId, string name, string? description, string code, bool isActive, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
    : TypeDocumentBaseDomainEvent(aggregateId, name, description, code, isActive, eventId, occurredAt, metadata)
{
    /// <summary>
    /// Creates the event as ms-catalogs publishes it.
    /// </summary>
    /// <param name="aggregateId">The identifier of the catalog entry.</param>
    /// <param name="name">The display name.</param>
    /// <param name="description">The description.</param>
    /// <param name="code">The catalog code.</param>
    /// <param name="isActive">Whether the type can be chosen.</param>
    /// <returns>The event.</returns>
    public static TypeDocumentCreatedDomainEvent Create(Guid aggregateId, string name, string? description, string code, bool isActive) => new(aggregateId, name, description, code, isActive);
}

/// <summary>
/// Copia local del cambio de un tipo de documento en ms-catalogs.
/// </summary>
[EventKey("TypeDocumentAggregate", 1, "TypeDocumentUpdatedDomainEvent", "ms-catalogs")]
public class TypeDocumentUpdatedDomainEvent(Guid aggregateId, string name, string? description, string code, bool isActive, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
    : TypeDocumentBaseDomainEvent(aggregateId, name, description, code, isActive, eventId, occurredAt, metadata)
{
    /// <summary>
    /// Creates the event as ms-catalogs publishes it.
    /// </summary>
    /// <param name="aggregateId">The identifier of the catalog entry.</param>
    /// <param name="name">The display name.</param>
    /// <param name="description">The description.</param>
    /// <param name="code">The catalog code.</param>
    /// <param name="isActive">Whether the type can be chosen.</param>
    /// <returns>The event.</returns>
    public static TypeDocumentUpdatedDomainEvent Create(Guid aggregateId, string name, string? description, string code, bool isActive) => new(aggregateId, name, description, code, isActive);
}

/// <summary>
/// Copia local del borrado de un tipo de documento en ms-catalogs.
/// </summary>
[EventKey("TypeDocumentAggregate", 1, "TypeDocumentDeletedDomainEvent", "ms-catalogs")]
public class TypeDocumentDeletedDomainEvent(Guid aggregateId, string name, string? description, string code, bool isActive, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
    : TypeDocumentBaseDomainEvent(aggregateId, name, description, code, isActive, eventId, occurredAt, metadata)
{
    /// <summary>
    /// Creates the event as ms-catalogs publishes it.
    /// </summary>
    /// <param name="aggregateId">The identifier of the catalog entry.</param>
    /// <param name="name">The display name.</param>
    /// <param name="description">The description.</param>
    /// <param name="code">The catalog code.</param>
    /// <param name="isActive">Whether the type can be chosen.</param>
    /// <returns>The event.</returns>
    public static TypeDocumentDeletedDomainEvent Create(Guid aggregateId, string name, string? description, string code, bool isActive) => new(aggregateId, name, description, code, isActive);
}
