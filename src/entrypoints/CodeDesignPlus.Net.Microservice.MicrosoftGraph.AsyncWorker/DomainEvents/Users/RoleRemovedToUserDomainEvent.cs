namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;

/// <summary>
/// Gemelo del evento de ms-users. Los nombres de los parametros del constructor coinciden con los de las
/// propiedades del original, o el valor viaja bien y se pierde al leerlo, sin error (regla 26 §4).
/// </summary>
[EventKey<UserAggregate>(1, "RoleRemovedToUserDomainEvent", "ms-users")]
public class RoleRemovedToUserDomainEvent(
     Guid aggregateId,
     string? displayName,
     Guid tenantId,
     Guid role,
     bool stillHasItElsewhere,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string? DisplayName { get; } = displayName;

    /// <summary>
    /// La copropiedad en la que el usuario deja de tener ese rol.
    /// </summary>
    public Guid TenantId { get; } = tenantId;

    /// <summary>
    /// El id del rol en el catalogo, que es el mismo en todos los entornos.
    /// </summary>
    public Guid Role { get; } = role;

    /// <summary>
    /// Si al usuario le queda ese mismo rol en alguna otra copropiedad.
    /// </summary>
    /// <remarks>
    /// <b>Es lo unico que impide sacarlo del grupo por error.</b> El grupo es global, asi que quitarle
    /// "Residente" en una copropiedad y sacarlo del grupo le quitaria el papel en todas. Lo calcula
    /// ms-users, que es el unico que tiene el documento entero.
    /// </remarks>
    public bool StillHasItElsewhere { get; } = stillHasItElsewhere;

    public static RoleRemovedToUserDomainEvent Create(Guid aggregateId, string? displayName, Guid tenantId, Guid role, bool stillHasItElsewhere)
    {
        return new RoleRemovedToUserDomainEvent(aggregateId, displayName, tenantId, role, stillHasItElsewhere);
    }
}
