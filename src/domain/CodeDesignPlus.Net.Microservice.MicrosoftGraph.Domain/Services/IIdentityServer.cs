using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

public interface IIdentityServer
{
    Task<List<Role>> GetGroupsAsync(CancellationToken cancellationToken);
    Task<Role> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Role> GetGroupByNameAsync(string name, CancellationToken cancellationToken);
    Task<Role> CreateGroupAsync(Role role, CancellationToken cancellationToken);
    Task<Role> UpdateGroupAsync(Guid id, Role role, CancellationToken cancellationToken);
    Task DeleteGroupAsync(Guid id, CancellationToken cancellationToken);

    Task<List<User>> GetUsersAsync(CancellationToken cancellationToken);
    Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateUserAsync(Guid id, User user, CancellationToken cancellationToken);
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken);

    Task AddUserToGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);
    Task RemoveUserFromGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);

    
    Task UpdateContactInfoAsync(Guid id, ContactInfo contact, CancellationToken cancellationToken);

    Task UpdateJobInfoAsync(Guid id, JobInfo job, CancellationToken cancellationToken);
}
