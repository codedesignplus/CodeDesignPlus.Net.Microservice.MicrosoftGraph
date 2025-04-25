using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.IdentityServer;
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
        }
    }
}
