
namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Repositories;

public class GraphRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<GraphRepository> logger)
    : RepositoryBase(serviceProvider, mongoOptions, logger), IRoleRepository
{
    public Task<RoleAggregate> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var collection = base.GetCollection<RoleAggregate>();

        var filter = Builders<RoleAggregate>.Filter.Eq(x => x.Name, name);

        return collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}