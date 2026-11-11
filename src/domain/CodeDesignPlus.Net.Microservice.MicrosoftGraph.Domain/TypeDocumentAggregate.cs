using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;

/// <summary>
/// Copia local de un tipo de documento del catalogo de ms-catalogs, que es su dueno (pendings/663).
/// </summary>
/// <remarks>
/// El registro de Entra External ID solo trae el codigo que eligio la persona en la lista desplegable (<c>CC</c>,
/// <c>NIT</c>); con esta copia se arma el <see cref="DocumentType"/> completo sin llamar a ms-catalogs en
/// medio del registro. Se llena con los eventos del catalogo y nunca se edita aqui.
/// </remarks>
public class TypeDocumentAggregate(Guid id) : AggregateRootBase(id)
{
    /// <summary>
    /// Gets the display name of the type, e.g. "Cédula de Ciudadanía".
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Gets the catalog code, e.g. "CC", already normalized by ms-catalogs.
    /// </summary>
    public string Code { get; private set; } = null!;

    /// <summary>
    /// Gets when ms-catalogs produced the event this copy comes from; an older event never overwrites a newer one.
    /// </summary>
    public Instant LastEventAt { get; private set; }

    /// <summary>
    /// Creates the local copy of a catalog entry as announced by ms-catalogs.
    /// </summary>
    /// <param name="id">The identifier of the entry in ms-catalogs.</param>
    /// <param name="name">The display name.</param>
    /// <param name="code">The catalog code.</param>
    /// <param name="isActive">Whether the entry can be chosen.</param>
    /// <param name="isDeleted">Whether the entry was deleted in ms-catalogs.</param>
    /// <param name="occurredAt">When ms-catalogs produced the event.</param>
    /// <returns>The local copy.</returns>
    public static TypeDocumentAggregate Create(Guid id, string name, string code, bool isActive, bool isDeleted, Instant occurredAt)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdIsInvalid);
        DomainGuard.IsNullOrEmpty(name, Errors.NameRequired);
        DomainGuard.IsNullOrWhiteSpace(code, Errors.DocumentTypeIsRequired);

        return new TypeDocumentAggregate(id)
        {
            Name = name,
            Code = NormalizeCode(code),
            IsActive = isActive && !isDeleted,
            IsDeleted = isDeleted,
            LastEventAt = occurredAt,
            CreatedAt = occurredAt,
        };
    }

    /// <summary>
    /// Builds the reference that travels in the user.
    /// </summary>
    /// <returns>The document type with the id, the display name and the code of this entry.</returns>
    public DocumentType ToDocumentType() => new(Id, Name, Code);

    /// <summary>
    /// Normaliza un codigo como lo hace ms-catalogs: sin espacios y en mayusculas, asi <c>cc</c> del registro encuentra
    /// <c>CC</c> del catalogo.
    /// </summary>
    /// <param name="code">The code as typed or configured.</param>
    /// <returns>The normalized code.</returns>
    public static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();
}
