using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Helpers.Services;

public class IdentityServerFake : IIdentityServer
{
    public Role Group { get; set; } = null!;
    public User User { get; set; } = null!;
    public ContactInfo ContactInfo { get; set; } = null!;
    public JobInfo JobInfo { get; set; } = null!;

    public Dictionary<string, object[]> CreateUserInvokeHistory { get; } = [];
    public Dictionary<string, object[]> AddUserToGroupInvokeHistory { get; } = [];
    public Dictionary<string, object[]> CreateGroupInvokeHistory { get; } = [];
    public Dictionary<string, object[]> DeleteGroupInvokeHistory { get; } = [];
    public Dictionary<string, object[]> DeleteUserInvokeHistory { get; } = [];
    public Dictionary<string, object[]> GetGroupByIdInvokeHistory { get; } = [];
    public Dictionary<string, object[]> GetGroupByNameInvokeHistory { get; } = [];
    public Dictionary<string, object[]> GetGroupsInvokeHistory { get; } = [];
    public Dictionary<string, object[]> GetUserByIdInvokeHistory { get; } = [];
    public Dictionary<string, object[]> GetUserByEmailInvokeHistory { get; } = [];
    public Dictionary<string, object[]> GetUsersInvokeHistory { get; } = [];
    public Dictionary<string, object[]> RemoveUserFromGroupInvokeHistory { get; } = [];
    public Dictionary<string, object[]> UpdateContactInfoInvokeHistory { get; } = [];
    public Dictionary<string, object[]> UpdateGroupInvokeHistory { get; } = [];
    public Dictionary<string, object[]> UpdateJobInfoInvokeHistory { get; } = [];
    public Dictionary<string, object[]> UpdateUserInvokeHistory { get; } = [];
    public Dictionary<string, object[]> UpdateUserPhoneInvokeHistory { get; } = [];

    public void SetValues(Role role, User user, ContactInfo contactInfo, JobInfo jobInfo)
    {
        Group = role;
        User = user;
        ContactInfo = contactInfo;
        JobInfo = jobInfo;
    }

    public Task AddUserToGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
    {
        AddUserToGroupInvokeHistory.Add(userId.ToString(), [userId, groupId, cancellationToken]);

        return Task.CompletedTask;
    }

    public Task<Role> CreateGroupAsync(Role role, CancellationToken cancellationToken)
    {
        CreateGroupInvokeHistory.Add(role.Id.ToString(), [role, cancellationToken]);

        return Task.FromResult(role);
    }

    public Task DeleteGroupAsync(Guid id, CancellationToken cancellationToken)
    {
        DeleteGroupInvokeHistory.Add(id.ToString(), [id, cancellationToken]);

        return Task.CompletedTask;
    }

    public Task DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        DeleteUserInvokeHistory.Add(id.ToString(), [id, cancellationToken]);

        return Task.CompletedTask;
    }

    public Task<Role> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        GetGroupByIdInvokeHistory.Add(id.ToString(), [id, cancellationToken]);

        if (Group != null && Group.Id == id)
            return Task.FromResult(Group);

        return null!;
    }

    public Task<Role> GetGroupByNameAsync(string name, CancellationToken cancellationToken)
    {
        GetGroupByNameInvokeHistory.Add(name, [name, cancellationToken]);

        if (Group != null && Group.Name == name)
            return Task.FromResult(Group);

        return null!;
    }

    public Task<List<Role>> GetGroupsAsync(CancellationToken cancellationToken)
    {
        GetGroupsInvokeHistory.Add("GetGroups", [cancellationToken]);

        if (Group != null)
            return Task.FromResult(new List<Role>() { Group });

        return Task.FromResult((List<Role>)null!);
    }

    public Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        GetUserByIdInvokeHistory.Add(id.ToString(), [id, cancellationToken]);

        if (User != null && User.Id == id)
            return Task.FromResult(User);

        return null!;
    }

    public Task<List<User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        GetUsersInvokeHistory.Add("GetUsers", [cancellationToken]);

        if (User != null)
            return Task.FromResult(new List<User>() { User });

        return Task.FromResult((List<User>)null!);
    }

    public Task RemoveUserFromGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
    {
        RemoveUserFromGroupInvokeHistory.Add(userId.ToString(), [userId, groupId, cancellationToken]);

        return Task.CompletedTask;
    }

    public Task UpdateContactInfoAsync(Guid id, ContactInfo contact, CancellationToken cancellationToken)
    {
        UpdateContactInfoInvokeHistory.Add(id.ToString(), [id, contact, cancellationToken]);

        return Task.CompletedTask;
    }

    public Task<Role> UpdateGroupAsync(Guid id, Role role, CancellationToken cancellationToken)
    {
        UpdateGroupInvokeHistory.Add(id.ToString(), [id, role, cancellationToken]);

        if (Group != null && Group.Id == id)
            return Task.FromResult(role);

        return null!;
    }

    public Task UpdateJobInfoAsync(Guid id, JobInfo job, CancellationToken cancellationToken)
    {
        UpdateJobInfoInvokeHistory.Add(id.ToString(), [id, job, cancellationToken]);

        return Task.CompletedTask;
    }

    public Task UpdateUserAsync(Guid id, User user, CancellationToken cancellationToken)
    {
        UpdateUserInvokeHistory.Add(id.ToString(), [id, user, cancellationToken]);

        if (User != null && User.Id == id)
            return Task.FromResult(user);

        return null!;
    }

    public Task<Guid> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        CreateUserInvokeHistory.Add(user.Id.ToString(), [user, cancellationToken]);

        if (User != null && User.Id == user.Id)
            return Task.FromResult(user.Id);

        return null!;
    }

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        GetUserByIdInvokeHistory.Add(email, [email, cancellationToken]);

        if (User != null && User.Email == email)
            return Task.FromResult(User)!;

        return null!;
    }


    public Task UpdateUserPhoneAsync(Guid id, string phone, CancellationToken cancellationToken)
    {
        UpdateUserPhoneInvokeHistory.Add(id.ToString(), [id, phone, cancellationToken]);

        if (User != null && User.Id == id)
        {
            User.Phone = phone;
            return Task.FromResult(User);
        }

        return null!;
    }
}
