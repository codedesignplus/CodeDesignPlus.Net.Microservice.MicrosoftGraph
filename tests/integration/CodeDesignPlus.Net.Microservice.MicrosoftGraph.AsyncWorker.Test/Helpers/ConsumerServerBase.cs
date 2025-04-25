using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using Moq;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Helpers;

public abstract class ConsumerServerBase : ServerBase<Program>, IClassFixture<Server<Program>>
{
    private const int maxRetries = 3;

    public Mock<IIdentityServer> IdentityServerMock { get; set; } = new Mock<IIdentityServer>();

    public ConsumerServerBase(Server<Program> server) : base(server)
    {
        server.InMemoryCollection = (x) =>
        {
            x.Add("Vault:Enable", "false");
            x.Add("Vault:Address", "http://localhost:8200");
            x.Add("Vault:Token", "root");
            x.Add("Solution", "CodeDesignPlus");
            x.Add("AppName", "my-test");
            x.Add("RabbitMQ:UserName", "guest");
            x.Add("RabbitMQ:Password", "guest");
            x.Add("Security:ValidAudiences:0", Guid.NewGuid().ToString());
        };

        server.ConfigureServices = ConfigureServices;
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var identityServerDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IIdentityServer));

        if (identityServerDescriptor != null)
            services.Remove(identityServerDescriptor);

        services.AddSingleton<IIdentityServer>(this.IdentityServerMock.Object);
    }

    protected static async Task<T> Retry<T>(Func<Task<T>> process)
    {
        T item = default!;

        var currentRetry = 0;

        do
        {
            item = await process();

            if (item == null)
            {
                currentRetry++;
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        } while (item == null && currentRetry < maxRetries);

        return item;
    }
}
