using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

public interface IIdentityServer
{
    Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken);
    Task<Role> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Role> GetGroupByNameAsync(string name, CancellationToken cancellationToken);
    Task<Role> CreateGroupAsync(Role role, CancellationToken cancellationToken);
    Task<Role> UpdateRoleAsync(Guid id, Role role, CancellationToken cancellationToken);
    Task DeleteGroupAsync(Guid id, CancellationToken cancellationToken);

    Task<List<User>> GetUsersAsync(CancellationToken cancellationToken);
    Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken);
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken);
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken);
}
