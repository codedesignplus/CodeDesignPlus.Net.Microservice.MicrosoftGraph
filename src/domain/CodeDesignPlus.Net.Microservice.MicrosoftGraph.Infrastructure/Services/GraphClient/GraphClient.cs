using Microsoft.Graph;
using Azure.Identity;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;

public class GraphClient : IGraphClient
{
    public GraphServiceClient Client { get; private set; } = null!;

    public GraphClient(IOptions<GraphOptions> graphOptions)
    {
        // The client credentials flow requires that you request the
        // /.default scope, and pre-configure your permissions on the
        // app registration in Azure. An administrator must grant consent
        // to those permissions beforehand.
        var scopes = new[] { "https://graph.microsoft.com/.default" };


        // using Azure.Identity;
        var options = new ClientSecretCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
        var clientSecretCredential = new ClientSecretCredential(
            graphOptions.Value.TenantId, graphOptions.Value.ClientId, graphOptions.Value.ClientSecret, options);

        var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

        this.Client = graphClient;
    }
}
