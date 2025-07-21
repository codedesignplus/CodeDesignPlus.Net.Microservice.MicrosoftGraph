
namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Repositories;

public class UserRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UserRepository> logger)
    : RepositoryBase(serviceProvider, mongoOptions, logger), IUserRepository
{
    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
    {
        var item = await this.GetCollection<UserAggregate>().FindAsync(x => x.Email == email, cancellationToken: cancellationToken);

        return await item.AnyAsync(cancellationToken);
    }
   

    public Task<UserCiamAggregate> GetByIdentityProviderId(Guid id, CancellationToken cancellationToken)
    {
        var collection = GetCollection<UserCiamAggregate>();

        return collection.Find(x => x.Id == id && x.IsActive).FirstOrDefaultAsync(cancellationToken);
    }
}