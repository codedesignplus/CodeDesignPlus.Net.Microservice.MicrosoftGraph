using Microsoft.Graph;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Test.Services.GraphClient;

public class GraphClientTest
{
    [Fact]
    public void Constructor_ValidOptions_InitializesGraphServiceClient()
    {
        // Arrange
        var mockOptions = new Mock<IOptions<GraphOptions>>();
        var graphOptions = new GraphOptions
        {
            TenantId = "test-tenant-id",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            Scopes = ["https://graph.microsoft.com/.default"]
        };
        mockOptions.Setup(o => o.Value).Returns(graphOptions);

        // Act
        var graphClient = new Infrastructure.Services.GraphClient.GraphClient(mockOptions.Object);

        // Assert
        Assert.NotNull(graphClient.Client);
        Assert.IsType<GraphServiceClient>(graphClient.Client);
    }

    [Fact]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        IOptions<GraphOptions> nullOptions = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Infrastructure.Services.GraphClient.GraphClient(nullOptions));
    }
}
