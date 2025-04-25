using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.DeleteGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Helpers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using Moq;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class DeleteGroupInMicrosoftGraphHandlerTest(Server<Program> server) : ConsumerServerBase(server)
{
    [Fact]
    public async Task Pusblish_Consumer_Success()
    {
        // Arrange
        var roleAggregate = RoleAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), "Admin", "This role is for admin", true);
        var roleRepository = this.Services.GetRequiredService<IRoleRepository>();
        var pubsub = this.Services.GetRequiredService<IPubSub>();
        var domainEvent = new RoleDeletedDomainEvent(roleAggregate.Id, "Admin", "This role is for admin", true);

        var group = new Domain.Models.Role
        {
            Id = roleAggregate.IdIdentityServer,
            Name = "Admin",
            Description = "This role is for admin",
            IsActive = true,
        };

        this.IdentityServerMock
            .Setup(x => x.GetGroupByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        this.IdentityServerMock
            .Setup(x => x.DeleteGroupAsync(group.Id, It.IsAny<CancellationToken>()));

        await roleRepository.CreateAsync(roleAggregate, CancellationToken.None);

        // Act
        _ = pubsub.PublishAsync(domainEvent, CancellationToken.None);

        // Assert
        var role = await Retry(async () => {
            var item = await roleRepository.FindAsync<RoleAggregate>(domainEvent.AggregateId, CancellationToken.None);

            if(item.Name != domainEvent.Name)
                return null;

            return item;
        });

        Assert.NotNull(role);
        Assert.Equal(domainEvent.Name, role.Name);
        Assert.Equal(domainEvent.Description, role.Description);
        Assert.Equal(domainEvent.IsActive, role.IsActive);
        Assert.Equal(group.Id, role.IdIdentityServer);

        this.IdentityServerMock.Verify(m => m.DeleteGroupAsync(group.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
