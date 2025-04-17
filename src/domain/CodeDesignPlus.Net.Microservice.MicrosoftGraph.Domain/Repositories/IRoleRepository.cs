namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;

public interface IRoleRepository : IRepositoryBase
{
    public Task<RoleAggregate> GetByNameAsync(string name, CancellationToken cancellationToken);
}