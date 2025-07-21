
namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Repositories;

public class UserRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UserRepository> logger)
    : RepositoryBase(serviceProvider, mongoOptions, logger), IUserRepository
{
    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
    {
        var item = await this.GetCollection<UserAggregate>().FindAsync(x => x.Email == email, cancellationToken: cancellationToken);

        return await item.AnyAsync(cancellationToken);
    }
   

    public Task<UserAggregate> GetByIdentityProviderId(Guid id, CancellationToken cancellationToken)
    {
        var collection = GetCollection<UserAggregate>();

        return collection.Find(x => x.IdentityProviderId == id && x.IsActive).FirstOrDefaultAsync(cancellationToken);
    }
}