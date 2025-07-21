namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;

public interface IUserRepository : IRepositoryBase
{
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken);
    Task<UserAggregate> GetByIdentityProviderId(Guid id, CancellationToken cancellationToken);
}