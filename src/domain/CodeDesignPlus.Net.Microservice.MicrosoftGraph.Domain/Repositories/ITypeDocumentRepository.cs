namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;

/// <summary>
/// Copia local de los tipos de documento de ms-catalogs (pendings/663).
/// </summary>
public interface ITypeDocumentRepository : IRepositoryBase
{
    /// <summary>
    /// Guarda la copia de un tipo tal como la anuncia ms-catalogs, salvo que ya haya una de un evento posterior.
    /// </summary>
    /// <remarks>
    /// Los eventos de crear, actualizar y borrar llegan por colas distintas y en cualquier orden; reprocesar uno no cambia
    /// nada.
    /// </remarks>
    /// <param name="typeDocument">The copy built from the event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the copy was written; <c>false</c> if a newer one was already stored.</returns>
    Task<bool> ApplyAsync(TypeDocumentAggregate typeDocument, CancellationToken cancellationToken);

    /// <summary>
    /// Busca el tipo activo con ese codigo.
    /// </summary>
    /// <param name="code">The catalog code, in any case and with or without surrounding spaces.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The active entry, or <c>null</c> if no active entry uses that code.</returns>
    Task<TypeDocumentAggregate?> FindActiveByCodeAsync(string code, CancellationToken cancellationToken);
}
