
namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Repositories;

public class UserCiamRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<IUserCiamRepository> logger)
    : RepositoryBase(serviceProvider, mongoOptions, logger), IUserCiamRepository
{
    public Task<List<UserCiamAggregate>> GetUsersPendingReplicateAsync(CancellationToken cancellationToken)
    {
        var collection = GetCollection<UserCiamAggregate>();

        return collection.Find(x => !x.UserReplicated).ToListAsync(cancellationToken);
    }

    public Task<UserCiamAggregate> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var collection = GetCollection<UserCiamAggregate>();

        return collection.Find(x => x.Email == email && x.IsActive).FirstOrDefaultAsync(cancellationToken);
    }
}