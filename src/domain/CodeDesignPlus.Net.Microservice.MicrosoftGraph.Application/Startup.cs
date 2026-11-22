using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application
{
    public class Startup : IStartup
    {
        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {
            MapsterConfigGraph.Configure();

            services.AddSingleton<IValidateOptions<UserDocumentOptions>, UserDocumentOptionsValidator>();

            services.AddOptions<UserDocumentOptions>()
                .Bind(configuration.GetSection(UserDocumentOptions.Section))
                .ValidateOnStart();
        }
    }
}
