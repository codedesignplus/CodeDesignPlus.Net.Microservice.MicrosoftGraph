namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;

public interface IUserCiamRepository : IRepositoryBase
{
    Task<List<UserCiamAggregate>> GetUsersPendingReplicateAsync(CancellationToken cancellationToken);
}