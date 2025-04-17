using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;
using MapsterMapper;
using Microsoft.Graph.Connections.Item.Groups;
using Microsoft.Graph.Models;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.IdentityServer;

public class IdentityServer(IGraphClient graph, IMapper mapper, ILogger<IdentityServer> logger) : IIdentityServer
{
    public async Task<List<Role>> GetGroupsAsync(CancellationToken cancellationToken)
    {
        var response = await graph.Client.Groups.GetAsync(cancellationToken: cancellationToken);

        var groups = mapper.Map<List<Role>>(response?.Value ?? []);

        return groups;
    }

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

    public async Task<Role> GetGroupByNameAsync(string name, CancellationToken cancellationToken)
    {
        var response = await graph.Client.Groups.GetAsync(cancellationToken: cancellationToken);

        var group = response?.Value?.FirstOrDefault(x => x.DisplayName == name);

        if (group == null)
            return null!;

        var role = mapper.Map<Role>(group);

        return role;
    }

    public async Task<Role> CreateGroupAsync(Role role, CancellationToken cancellationToken)
    {
        var group = new Group
        {
            Id = role.Id.ToString(),
            DisplayName = role.Name,
            Description = role.Description,
            MailEnabled = false,
            MailNickname = role.Name,
            SecurityEnabled = true
        };

        var response = await graph.Client.Groups.PostAsync(group, cancellationToken: cancellationToken);

        if (response == null)
            return null!;

        return mapper.Map<Role>(response);
    }

    public async Task<Role> UpdateGroupAsync(Guid id, Role role, CancellationToken cancellationToken)
    {
        var group = new Group
        {
            DisplayName = role.Name,
            Description = role.Description,
            MailEnabled = false,
            MailNickname = role.Name,
            SecurityEnabled = true
        };

        var response = await graph.Client.Groups[id.ToString()].PatchAsync(group, cancellationToken: cancellationToken);

        if (response == null)
            return null!;

        return mapper.Map<Role>(response);
    }

    public async Task DeleteGroupAsync(Guid id, CancellationToken cancellationToken)
    {
        await graph.Client.Groups[id.ToString()].DeleteAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<Domain.Models.User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await graph.Client.Users.GetAsync(cancellationToken: cancellationToken);

        return mapper.Map<List<Domain.Models.User>>(users?.Value ?? []);
    }

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

            var user = mapper.Map<Domain.Models.User>(response);

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user by id: {Id}", id);
        }

        return null!;
    }

    public Task UpdateUserAsync(Guid id, Domain.Models.User user, CancellationToken cancellationToken)
    {
        var updateUser = new Microsoft.Graph.Models.User
        {
            DisplayName = user.DisplayName,
            GivenName = user.FirstName,
            Surname = user.LastName,
            MobilePhone = user.PhoneNumber,
            AccountEnabled = true
        };

        return graph.Client.Users[id.ToString()].PatchAsync(updateUser, cancellationToken: cancellationToken);
    }

    public Task DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        return graph.Client.Users[id.ToString()].DeleteAsync(cancellationToken: cancellationToken);
    }

    public Task AddUserToGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
    {
        var requestBody = new ReferenceCreate
        {
            OdataId = $"https://graph.microsoft.com/v1.0/directoryObjects/{userId}",
        };

        return graph.Client.Groups[groupId.ToString()].Members.Ref.PostAsync(requestBody, cancellationToken: cancellationToken);
    }

    public Task RemoveUserFromGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
    {
        return graph.Client.Groups[groupId.ToString()].Members[userId.ToString()].Ref.DeleteAsync(cancellationToken: cancellationToken);
    }

    public Task UpdateContactInfoAsync(Guid id, ContactInfo contact, CancellationToken cancellationToken)
    {
        var updateUser = new Microsoft.Graph.Models.User
        {
            StreetAddress = contact.Address,
            City = contact.City,
            State = contact.State,
            Country = contact.Country,
            PostalCode = contact.PostalCode,
            BusinessPhones = contact.Phone != null ? [contact.Phone] : null,
            Mail = contact.Email?[0],
            OtherMails = contact.Email?[1..].ToList() ?? []
        };

        return graph.Client.Users[id.ToString()].PatchAsync(updateUser, cancellationToken: cancellationToken);
    }

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
