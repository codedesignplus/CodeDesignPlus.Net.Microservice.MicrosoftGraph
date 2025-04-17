namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Repositories;

public class UserRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UserRepository> logger) 
    : RepositoryBase(serviceProvider, mongoOptions, logger), IUserRepository
{
   
}