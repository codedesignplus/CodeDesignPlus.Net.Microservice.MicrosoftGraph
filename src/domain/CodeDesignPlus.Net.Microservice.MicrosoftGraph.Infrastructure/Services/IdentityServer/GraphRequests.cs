using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;
using Microsoft.Graph.Connections.Item.Groups;
using Microsoft.Graph.Models;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.IdentityServer;

public class IdentityServer(IGraphClient graph) : IIdentityServer
{
    public async Task<List<Group>> GetGroupsAsync()
    {
        var groups = await graph.Client.Groups.GetAsync();

        return groups?.Value ?? [];
    }

    public async Task GetUsers()
    {
        var users = await graph.Client.Users.GetAsync();
    }
    public async Task GetUser(string userId)
    {
        var user = await graph.Client.Users[userId].GetAsync();
    }
    public async Task GetUserGroups(string userId)
    {
        var groups = await graph.Client.Users[userId].MemberOf.GetAsync();
    }
    public async Task GetGroup(string groupId)
    {
        var group = await graph.Client.Groups[groupId].GetAsync();
    }
    public async Task GetGroupMembers(string groupId)
    {
        var members = await graph.Client.Groups[groupId].Members.GetAsync();
    }
    public async Task CreateGroup(string groupName)
    {
        var group = new Group
        {
            DisplayName = groupName,
            MailEnabled = false,
            MailNickname = groupName,
            SecurityEnabled = true
        };
        await graph.Client.Groups.PostAsync(group);
    }
    public async Task DeleteGroup(string groupId)
    {
        await graph.Client.Groups[groupId].DeleteAsync();
    }
    public async Task UpdateGroup(string groupId, string newGroupName)
    {
        var group = new Group
        {
            DisplayName = newGroupName,
            MailEnabled = false,
            MailNickname = newGroupName,
            SecurityEnabled = true
        };
        await graph.Client.Groups[groupId].PatchAsync(group);
    }
    public Task AddUserToGroup(string userId, string groupId)
    {
        var member = new DirectoryObject
        {
            Id = userId
        };

        return Task.CompletedTask;
    }

    public Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Role> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Role> GetGroupByNameAsync(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Role> CreateGroupAsync(Role role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Role> UpdateRoleAsync(Guid id, Role role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteGroupAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<Domain.Models.User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Models.User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Models.User> CreateUserAsync(Domain.Models.User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Models.User> UpdateUserAsync(Domain.Models.User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
