using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

/// <summary>
/// Represents the interface for managing identity server operations, including groups, users, and their relationships.
/// </summary>
public interface IIdentityServer
{
    /// <summary>
    /// Retrieves a list of all groups.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of roles.</returns>
    Task<List<Role>> GetGroupsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the role.</returns>
    Task<Role> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a group by its name.
    /// </summary>
    /// <param name="name">The name of the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the role.</returns>
    Task<Role> GetGroupByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new group.
    /// </summary>
    /// <param name="role">The role object representing the group to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created role.</returns>
    Task<Role> CreateGroupAsync(Role role, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing group.
    /// </summary>
    /// <param name="id">The unique identifier of the group to update.</param>
    /// <param name="role">The updated role object.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated role.</returns>
    Task<Role> UpdateGroupAsync(Guid id, Role role, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the group to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteGroupAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a list of all users.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of users.</returns>
    Task<List<User>> GetUsersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user.</returns>
    Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="user">The updated user object.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateUserAsync(Guid id, User user, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a user to a group.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddUserToGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a user from a group.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveUserFromGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the contact information of a user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="contact">The updated contact information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateContactInfoAsync(Guid id, ContactInfo contact, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the job information of a user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="job">The updated job information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateJobInfoAsync(Guid id, JobInfo job, CancellationToken cancellationToken);
}
