
namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Repositories;

public class UserRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UserRepository> logger)
    : RepositoryBase(serviceProvider, mongoOptions, logger), IUserRepository
{
    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
    {
        var item = await this.GetCollection<UserAggregate>().FindAsync(x => x.Email == email, cancellationToken: cancellationToken);

        return await item.AnyAsync(cancellationToken);
    }


    public Task<UserAggregate> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var collection = GetCollection<UserAggregate>();

        return collection.Find(x => x.Email == email).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<UserAggregate> GetByIdentityProviderId(Guid id, CancellationToken cancellationToken)
    {
        var collection = GetCollection<UserAggregate>();

        return collection.Find(x => x.IdentityProviderId == id && x.IsActive).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> AddRoleAsync(Guid id, Guid idRoleIdentityServer, CancellationToken cancellationToken)
    {
        var filter = Builders<UserAggregate>.Filter.Eq(x => x.Id, id);

        // AddToSet anade sobre el estado real del documento y no duplica: dos asignaciones concurrentes anaden
        // cada una la suya y ambas sobreviven. Reescribir el documento entero hacia que el ultimo ganara.
        // No se toca UpdatedBy porque aqui no hay actor: el comando llega de un consumidor de eventos y nadie
        // firma la operacion; inventar un identificador seria mentir sobre quien la hizo.
        var update = Builders<UserAggregate>.Update
            .AddToSet(x => x.IdRoles, idRoleIdentityServer)
            .Set(x => x.UpdatedAt, SystemClock.Instance.GetCurrentInstant());

        var result = await GetCollection<UserAggregate>()
            .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        // ModifiedCount en cero significa que el grupo ya estaba: no es un fallo, es que no habia nada que hacer.
        return result.ModifiedCount > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveRoleAsync(Guid id, Guid idRoleIdentityServer, CancellationToken cancellationToken)
    {
        var filter = Builders<UserAggregate>.Filter.Eq(x => x.Id, id);

        var update = Builders<UserAggregate>.Update
            .Pull(x => x.IdRoles, idRoleIdentityServer)
            .Set(x => x.UpdatedAt, SystemClock.Instance.GetCurrentInstant());

        var result = await GetCollection<UserAggregate>()
            .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        // Cero modificaciones significa que el grupo no estaba: quitar lo que ya no esta no es un error.
        return result.ModifiedCount > 0;
    }
}
