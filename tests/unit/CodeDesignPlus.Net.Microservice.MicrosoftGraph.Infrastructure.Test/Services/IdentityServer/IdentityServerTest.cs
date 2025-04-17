using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;
using IS = CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.IdentityServer;
using GC = CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;


namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Test.Services.IdentityServer;

public class IdentityServerTest
{
    [Fact]
    public async Task TestGetRoles()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new GraphOptions()
        {
        });
        var graph = new GC.GraphClient(options);



        var graphRequests = new IS.IdentityServer(graph);

        // Act
        await graphRequests.GetGroupsAsync();

        // Assert
    }
}
