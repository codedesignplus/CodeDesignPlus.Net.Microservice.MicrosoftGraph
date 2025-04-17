using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;
using IS = CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.IdentityServer;
using GC = CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;
using Mapster;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;
using Microsoft.Graph.Reports.GetEmailActivityCountsWithPeriod;
using Microsoft.AspNetCore.Identity;


namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Test.Services.IdentityServer;

public class IdentityServerTest
{
    [Fact]
    public async Task GetGroupsAsync()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GraphOptions()
        {
        });
        var graph = new GC.GraphClient(options);

        var config = TypeAdapterConfig.GlobalSettings;

        config.NewConfig<Microsoft.Graph.Models.Group, Role>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.DisplayName)
            .Map(dest => dest.Description, src => src.Description);

        var mapper = new MapsterMapper.Mapper(config);

        var identityServer = new IS.IdentityServer(graph, mapper, Mock.Of<ILogger<IS.IdentityServer>>());

        // Act
        var roleExpected = new Role()
        {
            Name = "Test",
            Description = "Test",
            IsActive = true
        };

        var deleteGroup = await identityServer.GetGroupByNameAsync(roleExpected.Name, CancellationToken.None);

        if (deleteGroup != null)
        {
            await identityServer.DeleteGroupAsync(deleteGroup.Id, CancellationToken.None);
        }

        var group = await identityServer.CreateGroupAsync(roleExpected, CancellationToken.None);

        var roleUpdate = new Role()
        {
            Name = "TestUpdate",
            Description = "TestUpdate",
            IsActive = true
        };

        await identityServer.UpdateGroupAsync(group.Id, roleUpdate, CancellationToken.None);

        var role = await identityServer.GetGroupByIdAsync(group.Id, CancellationToken.None);

        // var roles = await identityServer.GetGroupsAsync(CancellationToken.None);


        // Assert
        Assert.NotNull(role);
    }

    [Fact]
    public async Task Users()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new GraphOptions()
        {
        });
        var graph = new GC.GraphClient(options);

        var config = TypeAdapterConfig.GlobalSettings;

        config.NewConfig<Microsoft.Graph.Models.User, User>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.DisplayName, src => src.DisplayName)
            .Map(dest => dest.FirstName, src => src.GivenName)
            .Map(dest => dest.LastName, src => src.Surname)
            .Map(dest => dest.Email, src => GetEmail(src))
            .Map(dest => dest.PhoneNumber, src => src.MobilePhone)
            .Map(dest => dest.IsActive, src => src.AccountEnabled ?? false);

        var mapper = new MapsterMapper.Mapper(config);
        var identityServer = new IS.IdentityServer(graph, mapper, Mock.Of<ILogger<IS.IdentityServer>>());

        await identityServer.DeleteUserAsync(Guid.Parse("c1369517-2b30-47f0-820c-4cc4c044f815"), CancellationToken.None);


        var userUpdate = new User()
        {
            DisplayName = "Pablo Emilio Liscano Galindo",
            FirstName = "Pablo Emilio",
            LastName = "Liscano Galindo",
            Email = "pliscano@outlook.com",
            PhoneNumber = "3107545354",
            IsActive = true
        };

        //await identityServer.UpdateUserAsync(Guid.Parse("c1369517-2b30-47f0-820c-4cc4c044f815"), userUpdate, CancellationToken.None);

        var userResult = await identityServer.GetUserByIdAsync(Guid.Parse("c1369517-2b30-47f0-820c-4cc4c044f815"), CancellationToken.None);

    }

    [Fact]
    public async Task UserGroup()
    {

        var userId = Guid.Parse("6708012d-7505-4f4b-8fa9-aa86d593b6a0");
        var groupId = Guid.Parse("6386b4a7-d874-4d38-9cc4-3cbb93d8c360");

        var options = Microsoft.Extensions.Options.Options.Create(new GraphOptions()
        {
        });

        var config = TypeAdapterConfig.GlobalSettings;

        var mapper = new MapsterMapper.Mapper(config);

        var graph = new GC.GraphClient(options);

        var identityServer = new IS.IdentityServer(graph, mapper, Mock.Of<ILogger<IS.IdentityServer>>());

        await identityServer.AddUserToGroupAsync(userId, groupId, CancellationToken.None);

        await identityServer.RemoveUserFromGroupAsync(userId, groupId, CancellationToken.None);
    }

    private static string? GetEmail(Microsoft.Graph.Models.User src)
    {
        if (src.Mail != null)
            return src.Mail;

        if (src.Identities != null && src.Identities.Count > 0)
            return src.Identities.FirstOrDefault(x => x.SignInType == "emailAddress")?.IssuerAssignedId;

        if (src.UserPrincipalName != null)
            return src.UserPrincipalName;

        return null;
    }
}
