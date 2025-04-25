
namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Repositories;

public class RoleRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<RoleRepository> logger)
    : RepositoryBase(serviceProvider, mongoOptions, logger), IRoleRepository
{
    public Task<RoleAggregate> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var collection = base.GetCollection<RoleAggregate>();

        var filter = Builders<RoleAggregate>.Filter.Eq(x => x.Name, name);

        return collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}