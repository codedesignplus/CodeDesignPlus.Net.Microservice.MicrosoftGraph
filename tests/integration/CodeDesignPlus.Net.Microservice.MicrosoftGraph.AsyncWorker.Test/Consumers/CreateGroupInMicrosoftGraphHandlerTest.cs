using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Helpers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using Moq;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class CreateGroupInMicrosoftGraphHandlerTest(Server<Program> server) : ConsumerServerBase(server)
{
    [Fact]
    public async Task Pusblish_Consumer_Success()
    {
        // Arrange
        var roleRepository = this.Services.GetRequiredService<IRoleRepository>();
        var pubsub = this.Services.GetRequiredService<IPubSub>();
        var domainEvent = new RoleCreatedDomainEvent(Guid.NewGuid(), "Admin", "This role is for admin", true);

        var group = new Domain.Models.Role
        {
            Id = Guid.NewGuid(),
            Name = domainEvent.Name,
            Description = domainEvent.Description,
            IsActive = true,
        };

        this.IdentityServerMock
            .Setup(x => x.GetGroupByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Models.Role)null!);

        this.IdentityServerMock
            .Setup(x => x.CreateGroupAsync(It.IsAny<Domain.Models.Role>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        // Act
        _ = pubsub.PublishAsync(domainEvent, CancellationToken.None);

        // Assert
        var role = await Retry(() => roleRepository.FindAsync<RoleAggregate>(domainEvent.AggregateId, CancellationToken.None));

        Assert.NotNull(role);
        Assert.Equal(domainEvent.Name, role.Name);
        Assert.Equal(domainEvent.Description, role.Description);
        Assert.Equal(domainEvent.IsActive, role.IsActive);
        Assert.Equal(group.Id, role.IdIdentityServer);

        this.IdentityServerMock.Verify(m => m.CreateGroupAsync(It.Is<Domain.Models.Role>(role =>
          role.Id == domainEvent.AggregateId &&
          role.Name == domainEvent.Name &&
          role.Description == domainEvent.Description &&
          role.IsActive == domainEvent.IsActive
      ), It.IsAny<CancellationToken>()), Times.Once);

    }

}
