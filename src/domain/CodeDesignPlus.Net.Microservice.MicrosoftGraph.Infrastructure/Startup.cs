using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.IdentityServer;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.TypeDocumentCatalog;
using Microsoft.Graph;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure
{
    public class Startup : IStartup
    {
        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetRequiredSection(GraphOptions.Section);

            services.AddOptions<GraphOptions>()
                .Bind(section)
                .ValidateDataAnnotations();

            services.AddSingleton<IGraphClient, GraphClient>();
            services.AddSingleton<IIdentityServer, IdentityServer>();

            // Fuera de cualquier try a proposito: un pais sin tipos de documento debe tumbar el arranque (pendings/663).
            var typeDocuments = TypeDocumentCatalog.Load(configuration["Seed:Country"]);

            services.AddSingleton<ITypeDocumentCatalog>(new TypeDocumentCatalog(typeDocuments));
        }
    }
}
