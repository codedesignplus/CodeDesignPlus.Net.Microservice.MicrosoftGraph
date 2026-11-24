namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;

/// <summary>
/// Gemelo del evento de ms-users: la cuenta se desactivó porque al usuario no le quedó nada.
/// </summary>
/// <remarks>
/// Los nombres de los parámetros del constructor coinciden con los de las propiedades del original (regla 26 §4).
/// </remarks>
[EventKey<UserAggregate>(1, "UserDeactivatedDomainEvent", "ms-users")]
public class UserDeactivatedDomainEvent(
     Guid aggregateId,
     string? displayName,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    /// <summary>
    /// El nombre del usuario.
    /// </summary>
    public string? DisplayName { get; } = displayName;

    /// <summary>
    /// Crea el evento.
    /// </summary>
    /// <param name="aggregateId">El usuario.</param>
    /// <param name="displayName">Su nombre.</param>
    /// <returns>El evento.</returns>
    public static UserDeactivatedDomainEvent Create(Guid aggregateId, string? displayName)
    {
        return new UserDeactivatedDomainEvent(aggregateId, displayName);
    }
}

/// <summary>
/// Gemelo del evento de ms-users: la cuenta que se había desactivado por no tener nada recibió un rol y vuelve a estar
/// activa.
/// </summary>
[EventKey<UserAggregate>(1, "UserReactivatedDomainEvent", "ms-users")]
public class UserReactivatedDomainEvent(
     Guid aggregateId,
     string? displayName,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    /// <summary>
    /// El nombre del usuario.
    /// </summary>
    public string? DisplayName { get; } = displayName;

    /// <summary>
    /// Crea el evento.
    /// </summary>
    /// <param name="aggregateId">El usuario.</param>
    /// <param name="displayName">Su nombre.</param>
    /// <returns>El evento.</returns>
    public static UserReactivatedDomainEvent Create(Guid aggregateId, string? displayName)
    {
        return new UserReactivatedDomainEvent(aggregateId, displayName);
    }
}
