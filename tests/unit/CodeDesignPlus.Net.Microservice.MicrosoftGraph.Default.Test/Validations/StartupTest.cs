using Castle.Core.Configuration;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Default.Test.Validations;

/// <summary>
/// A class for validating startup services.
/// </summary>
public class StartupTest
{
    /// <summary>
    /// Validates that the startup services do not throw exceptions during initialization.
    /// </summary>
    [Theory]
    [Startup<Domain.Startup>]
    public void Sturtup_CheckNotThrowException_Domain(IStartup startup, Exception exception)
    {
        // Assert
        Assert.NotNull(startup);
        Assert.Null(exception);
    }

    /// <summary>
    /// Validates that the startup services do not throw exceptions during initialization.
    /// </summary>
    [Theory]
    [Startup<Application.Startup>()]
    public void Sturtup_CheckNotThrowException_Application(IStartup startup, Exception exception)
    {
        // Assert
        Assert.NotNull(startup);
        Assert.Null(exception);
    }

    /// <summary>
    /// Validates that the startup services do not throw exceptions during initialization.
    /// </summary>
    [Fact]
    public void Sturtup_CheckNotThrowException_RegisterServices()
    {
        var services = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Graph:ClientId", "test-client-id" },
            { "Graph:ClientSecret", "test-client-secret" },
            { "Graph:TenantId", "test-tenant-id" },
            { "Graph:BaseUrl", "https://graph.microsoft.com/v1.0/" }
        });

        var configuration = configurationBuilder.Build();

        // Act
        var startup = new Infrastructure.Startup();

        var exception = Record.Exception(() => startup.Initialize(services, configuration));

        // Assert
        Assert.Null(exception);

        var graphClient = services.FirstOrDefault(x => x.ServiceType == typeof(IGraphClient));
        var identityServer = services.FirstOrDefault(x => x.ServiceType == typeof(IIdentityServer));
        
        Assert.NotNull(graphClient);
        Assert.NotNull(identityServer);
        Assert.Equal(ServiceLifetime.Singleton, graphClient.Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, identityServer.Lifetime);
    }
}