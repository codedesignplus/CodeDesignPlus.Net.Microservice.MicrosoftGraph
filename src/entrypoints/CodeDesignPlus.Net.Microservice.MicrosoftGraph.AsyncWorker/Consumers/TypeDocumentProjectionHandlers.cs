using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Catalogs;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

/// <summary>
/// Lleva a la copia local los tipos de documento que anuncia ms-catalogs (pendings/663). Con ella el registro de Entra
/// External ID convierte el codigo elegido en la lista desplegable en el tipo completo.
/// </summary>
public static class TypeDocumentProjection
{
    /// <summary>
    /// Guarda la copia del tipo que trae el evento; uno anterior al guardado no cambia nada.
    /// </summary>
    /// <param name="repository">The local copy.</param>
    /// <param name="data">The event from ms-catalogs.</param>
    /// <param name="isDeleted">Whether the event announces a deletion.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The task.</returns>
    public static Task ApplyAsync(ITypeDocumentRepository repository, TypeDocumentBaseDomainEvent data, bool isDeleted, CancellationToken token)
    {
        var typeDocument = TypeDocumentAggregate.Create(data.AggregateId, data.Name, data.Code, data.IsActive, isDeleted, data.OccurredAt);

        return repository.ApplyAsync(typeDocument, token);
    }
}

/// <summary>
/// Un tipo nuevo en el catalogo se puede elegir al registrarse.
/// </summary>
[QueueName<TypeDocumentAggregate>("TypeDocumentCreatedProjectionHandler")]
public class TypeDocumentCreatedProjectionHandler(ITypeDocumentRepository repository) : IEventHandler<TypeDocumentCreatedDomainEvent>
{
    /// <inheritdoc/>
    public Task HandleAsync(TypeDocumentCreatedDomainEvent data, CancellationToken token) => TypeDocumentProjection.ApplyAsync(repository, data, false, token);
}

/// <summary>
/// Un cambio de nombre, codigo o estado en el catalogo se refleja en la copia.
/// </summary>
[QueueName<TypeDocumentAggregate>("TypeDocumentUpdatedProjectionHandler")]
public class TypeDocumentUpdatedProjectionHandler(ITypeDocumentRepository repository) : IEventHandler<TypeDocumentUpdatedDomainEvent>
{
    /// <inheritdoc/>
    public Task HandleAsync(TypeDocumentUpdatedDomainEvent data, CancellationToken token) => TypeDocumentProjection.ApplyAsync(repository, data, false, token);
}

/// <summary>
/// Un tipo borrado del catalogo deja de resolverse.
/// </summary>
[QueueName<TypeDocumentAggregate>("TypeDocumentDeletedProjectionHandler")]
public class TypeDocumentDeletedProjectionHandler(ITypeDocumentRepository repository) : IEventHandler<TypeDocumentDeletedDomainEvent>
{
    /// <inheritdoc/>
    public Task HandleAsync(TypeDocumentDeletedDomainEvent data, CancellationToken token) => TypeDocumentProjection.ApplyAsync(repository, data, true, token);
}
