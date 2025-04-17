namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Repositories;

public class GraphRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<GraphRepository> logger) 
    : RepositoryBase(serviceProvider, mongoOptions, logger), IRoleRepository
{
   
}