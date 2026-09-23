namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;

/// <summary>
/// Gemelo del evento de ms-users. Los nombres de los parametros del constructor coinciden con los de las
/// propiedades del original, o el valor viaja bien y se pierde al leerlo, sin error (regla 26 §4).
/// </summary>
[EventKey<UserAggregate>(1, "RoleAddedToUserDomainEvent", "ms-users")]
public class RoleAddedToUserDomainEvent(
     Guid aggregateId,
     string? displayName,
     Guid tenantId,
     Guid role,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string? DisplayName { get; } = displayName;

    /// <summary>
    /// La copropiedad en la que el usuario pasa a tener ese rol.
    /// </summary>
    /// <remarks>
    /// <b>Aqui se ignora a proposito.</b> Los grupos del proveedor de identidad son globales al
    /// directorio: no existe "Administrador de Malpelo XXI". Quien decide por copropiedad es el
    /// directorio de roles del SDK, que pregunta a ms-users; este micro solo mantiene la pertenencia al
    /// grupo, que es la union de todas.
    /// </remarks>
    public Guid TenantId { get; } = tenantId;

    /// <summary>
    /// El id del grupo del proveedor de identidad.
    /// </summary>
    public Guid Role { get; } = role;

    public static RoleAddedToUserDomainEvent Create(Guid aggregateId, string? displayName, Guid tenantId, Guid role)
    {
        return new RoleAddedToUserDomainEvent(aggregateId, displayName, tenantId, role);
    }
}
