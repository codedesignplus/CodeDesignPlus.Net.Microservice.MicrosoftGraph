using CodeDesignPlus.Net.Exceptions.Guards;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;
using MapsterMapper;
using Microsoft.Graph.Models;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.IdentityServer;

/// <summary>
/// Implementation of the IIdentityServer interface for managing identity server operations, including groups, users, and their relationships. 
/// </summary>
/// <param name="graph">The Graph client for interacting with Microsoft Graph API.</param>
/// <param name="mapper">The mapper for mapping between domain models and Graph models.</param>
/// <param name="logger">The logger for logging errors and information.</param>
/// <param name="graphOptions">The options for configuring the Graph client.</param>
public class IdentityServer(IGraphClient graph, IMapper mapper, ILogger<IdentityServer> logger, IOptions<GraphOptions> graphOptions) : IIdentityServer
{
    /// <summary>
    /// Retrieves a list of all groups.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of roles.</returns>
    public async Task<List<Role>> GetGroupsAsync(CancellationToken cancellationToken)
    {
        var response = await graph.Client.Groups.GetAsync(cancellationToken: cancellationToken);

        var groups = mapper.Map<List<Role>>(response?.Value ?? []);

        return groups;
    }

    /// <summary>
    /// Retrieves a group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the role.</returns>
    public async Task<Role> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await graph.Client.Groups[id.ToString()].GetAsync(cancellationToken: cancellationToken);

            if (response == null)
                return null!;

            var group = mapper.Map<Role>(response);

            return group;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting groups");
        }

        return null!;
    }

    /// <summary>
    /// Retrieves a group by its name.
    /// </summary>
    /// <param name="name">The name of the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the role.</returns>
    public async Task<Role> GetGroupByNameAsync(string name, CancellationToken cancellationToken)
    {
        var response = await graph.Client.Groups.GetAsync(cancellationToken: cancellationToken);

        var group = response?.Value?.FirstOrDefault(x => x.DisplayName == name);

        if (group == null)
            return null!;

        var role = mapper.Map<Role>(group);

        return role;
    }

    /// <summary>
    /// Creates a new group.
    /// </summary>
    /// <param name="role">The role object representing the group to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created role.</returns>
    public async Task<Role> CreateGroupAsync(Role role, CancellationToken cancellationToken)
    {
        var group = new Group
        {
            Id = role.Id.ToString(),
            DisplayName = role.Name,
            Description = role.Description,
            MailEnabled = false,
            MailNickname = role.Name.Replace(" ", "").ToLower(),
            SecurityEnabled = true
        };

        var response = await graph.Client.Groups.PostAsync(group, cancellationToken: cancellationToken);

        if (response == null)
            return null!;

        return mapper.Map<Role>(response);
    }

    /// <summary>
    /// Updates an existing group.
    /// </summary>
    /// <param name="id">The unique identifier of the group to update.</param>
    /// <param name="role">The updated role object.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated role.</returns>
    public async Task<Role> UpdateGroupAsync(Guid id, Role role, CancellationToken cancellationToken)
    {
        var group = new Group
        {
            DisplayName = role.Name,
            Description = role.Description,
            MailEnabled = false,
            MailNickname = role.Name.Replace(" ", "").ToLower(),
            SecurityEnabled = true
        };

        var response = await graph.Client.Groups[id.ToString()].PatchAsync(group, cancellationToken: cancellationToken);

        if (response == null)
            return null!;

        return mapper.Map<Role>(response);
    }

    /// <summary>
    /// Deletes a group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the group to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteGroupAsync(Guid id, CancellationToken cancellationToken)
    {
        await graph.Client.Groups[id.ToString()].DeleteAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of all users.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of users.</returns>
    public async Task<List<Domain.Models.User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await graph.Client.Users.GetAsync(cancellationToken: cancellationToken);

        return mapper.Map<List<Domain.Models.User>>(users?.Value ?? []);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user.</returns>
    public async Task<Domain.Models.User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await graph.Client.Users[id.ToString()].GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = ["id", "displayName", "givenName", "surname", "mobilePhone", "postalCode", "identities", "accountEnabled"];

            }, cancellationToken: cancellationToken);

            if (response == null)
                return null!;

            return new Domain.Models.User
            {
                Id = Guid.Parse(response.Id!),
                DisplayName = response.DisplayName!,
                FirstName = response.GivenName!,
                LastName = response.Surname!,
                Phone = response.MobilePhone!,
                Email = response.Identities?.FirstOrDefault()?.IssuerAssignedId ?? string.Empty,
                IsActive = response.AccountEnabled ?? false
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user by id: {Id}", id);
        }

        return null!;
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user.</returns>
    public async Task<Domain.Models.User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var response = await graph.Client.Users.GetAsync((requestConfiguration) =>
        {
            requestConfiguration.QueryParameters.Filter = $"mail eq '{email}'";
            requestConfiguration.QueryParameters.Select = ["id", "displayName", "givenName", "surname", "mobilePhone", "postalCode", "identities", "accountEnabled"];
        }, cancellationToken: cancellationToken);

        logger.LogWarning("Response from GetUserByEmailAsync: {@Response}", response);

        var user = response?.Value?.FirstOrDefault();

        if (user == null)
            return null!;

        return new Domain.Models.User
        {
            Id = Guid.Parse(user.Id!),
            DisplayName = user.DisplayName!,
            FirstName = user.GivenName!,
            LastName = user.Surname!,
            Phone = user.MobilePhone!,
            Email = user.Identities?.FirstOrDefault()?.IssuerAssignedId ?? string.Empty,
            IsActive = user.AccountEnabled ?? false
        };
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user object representing the user to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<Guid> CreateUserAsync(Domain.Models.User user, CancellationToken cancellationToken)
    {
        var mailNickname = user.Email.Split('@')[0];

        var newUser = new Microsoft.Graph.Models.User
        {
            Id = user.Id.ToString(),
            DisplayName = user.DisplayName,
            GivenName = user.FirstName,
            Surname = user.LastName,
            MobilePhone = user.Phone,
            AccountEnabled = user.IsActive,
            MailNickname = mailNickname,
            PasswordProfile = new PasswordProfile
            {
                ForceChangePasswordNextSignIn = true,
                Password = user.Password
            },
            PasswordPolicies = "DisablePasswordExpiration",
            CreationType = "LocalAccount",
            Identities =
            [
                new() {
                    SignInType = "emailAddress",
                    Issuer = graphOptions.Value.IssuerIdentity,
                    IssuerAssignedId = user.Email,
                },
            ],
        };

        var response = await graph.Client.Users.PostAsync(newUser, cancellationToken: cancellationToken);

        InfrastructureGuard.IsNull(response!, Errors.UserCreationFailed);

        return Guid.Parse(response!.Id!);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="user">The updated user object.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UpdateUserAsync(Guid id, Domain.Models.User user, CancellationToken cancellationToken)
    {
        var updateUser = new Microsoft.Graph.Models.User
        {
            DisplayName = user.DisplayName,
            GivenName = user.FirstName,
            Surname = user.LastName,
            MobilePhone = user.Phone,
            AccountEnabled = true
        };

        return graph.Client.Users[id.ToString()].PatchAsync(updateUser, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        return graph.Client.Users[id.ToString()].DeleteAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds a user to a group.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AddUserToGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
    {
        var requestBody = new ReferenceCreate
        {
            OdataId = $"https://graph.microsoft.com/v1.0/directoryObjects/{userId}",
        };

        return graph.Client.Groups[groupId.ToString()].Members.Ref.PostAsync(requestBody, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Removes a user from a group.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task RemoveUserFromGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
    {
        return graph.Client.Groups[groupId.ToString()].Members[userId.ToString()].Ref.DeleteAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates the contact information of a user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="contact">The updated contact information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UpdateContactInfoAsync(Guid id, ContactInfo contact, CancellationToken cancellationToken)
    {
        var updateUser = new Microsoft.Graph.Models.User
        {
            StreetAddress = contact.Address,
            City = contact.City,
            State = contact.State,
            Country = contact.Country,
            PostalCode = contact.ZipCode,
        };

        if (!string.IsNullOrEmpty(contact.Phone))
            updateUser.BusinessPhones = [contact.Phone];

        if (contact.Email != null)
            updateUser.Mail = contact.Email[0];

        if (contact.Email != null && contact.Email.Length > 1)
            updateUser.OtherMails = [.. contact.Email[1..]];

        return graph.Client.Users[id.ToString()].PatchAsync(updateUser, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates the job information of a user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="job">The updated job information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UpdateJobInfoAsync(Guid id, JobInfo job, CancellationToken cancellationToken)
    {
        var updateUser = new Microsoft.Graph.Models.User
        {
            JobTitle = job.JobTitle,
            CompanyName = job.CompanyName,
            Department = job.Department,
            EmployeeId = job.EmployeeId,
            EmployeeType = job.EmployeeType,
            OfficeLocation = job.OfficeLocation
        };

        return graph.Client.Users[id.ToString()].PatchAsync(updateUser, cancellationToken: cancellationToken);
    }
}
