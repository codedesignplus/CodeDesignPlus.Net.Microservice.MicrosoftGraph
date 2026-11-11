namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Repositories;

/// <summary>
/// Copia local de los tipos de documento de ms-catalogs (pendings/663).
/// </summary>
public class TypeDocumentRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<TypeDocumentRepository> logger)
    : RepositoryBase(serviceProvider, mongoOptions, logger), ITypeDocumentRepository
{
    private const int DuplicateKey = 11000;

    /// <inheritdoc/>
    public async Task<bool> ApplyAsync(TypeDocumentAggregate typeDocument, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(typeDocument);

        // Solo se reemplaza una copia de un evento anterior. Si la guardada es posterior, el filtro no la encuentra y el
        // upsert intenta insertar otra con el mismo _id: la base lo rechaza y eso significa que no habia nada que hacer.
        var filter = Builders<TypeDocumentAggregate>.Filter.And(
            Builders<TypeDocumentAggregate>.Filter.Eq(x => x.Id, typeDocument.Id),
            Builders<TypeDocumentAggregate>.Filter.Lt(x => x.LastEventAt, typeDocument.LastEventAt));

        try
        {
            await GetCollection<TypeDocumentAggregate>().ReplaceOneAsync(filter, typeDocument, new ReplaceOptions { IsUpsert = true }, cancellationToken);

            return true;
        }
        catch (MongoWriteException exception) when (exception.WriteError?.Code == DuplicateKey)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<TypeDocumentAggregate?> FindActiveByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var normalized = TypeDocumentAggregate.NormalizeCode(code);

        return await GetCollection<TypeDocumentAggregate>()
            .Find(x => x.Code == normalized && x.IsActive && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
