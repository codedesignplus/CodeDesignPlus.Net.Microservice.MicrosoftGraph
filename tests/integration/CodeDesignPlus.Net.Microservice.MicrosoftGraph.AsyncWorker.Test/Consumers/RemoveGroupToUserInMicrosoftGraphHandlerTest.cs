using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Helpers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using Moq;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class RemoveGroupToUserInMicrosoftGraphHandlerTest(Server<Program> server) : ConsumerServerBase(server)
{
    private readonly Domain.Models.ContactInfo contactInfo = new()
    {
        Address = "Street 123",
        City = "City",
        State = "State",
        Country = "Country",
        ZipCode = "12345",
        Phone = "3105631234",
        Email = ["joee.doe@fake.com"]
    };

    private readonly Domain.Models.JobInfo jobInfo = new()
    {
        JobTitle = "Software Engineer",
        CompanyName = "Tech Company",
        Department = "It",
        EmployeeId = "123456",
        EmployeeType = "Full Time",
        EmployHireDate = SystemClock.Instance.GetCurrentInstant(),
        OfficeLocation = "Remote"
    };

    [Fact]
    public async Task Pusblish_Consumer_Success()
    {
        // Arrange
        
        var userRepository = this.Services.GetRequiredService<IUserRepository>();
        var roleRepository = this.Services.GetRequiredService<IRoleRepository>();
        var pubsub = this.Services.GetRequiredService<IPubSub>();
        var roleAggregate = RoleAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), "Admin", "This role is for admin", true);
        var userAggregate = UserAggregate.Create(Guid.NewGuid(),"Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "123456", true);
        var domainEvent = new RoleRemovedToUserDomainEvent(userAggregate.Id, "Joe Doe", "Admin");

        userAggregate.AddRole(roleAggregate.IdIdentityServer);

        var userModel = new Domain.Models.User
        {
            Id = userAggregate.Id,
            FirstName = "Joe",
            LastName = "Doe",
            DisplayName = "Joe Doe",
            Email = "joe@fake.com",
            Phone = "3105631234",
            Contact = contactInfo,
            Job = jobInfo,
            IsActive = true,
        };

        var groupModel = new Domain.Models.Role
        {
            Id = roleAggregate.IdIdentityServer,
            Name = "Admin",
            Description = "This role is for admin",
            IsActive = true,
        };

        this.IdentityServerMock
            .Setup(x => x.GetUserByIdAsync(userModel.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userModel);

        this.IdentityServerMock
            .Setup(x => x.RemoveUserFromGroupAsync(userModel.Id, groupModel.Id, It.IsAny<CancellationToken>()));

        await roleRepository.CreateAsync(roleAggregate, CancellationToken.None);
        await userRepository.CreateAsync(userAggregate, CancellationToken.None);

        // Act
        _ = pubsub.PublishAsync(domainEvent, CancellationToken.None);

        // Assert
        var user = await Retry(async () => {
            var item = await userRepository.FindAsync<UserAggregate>(domainEvent.AggregateId, CancellationToken.None);

            if(item != null && item.IdRoles.Any(x => x == roleAggregate.IdIdentityServer))
                return null;

            return item;
        });

        Assert.NotNull(user);
        Assert.DoesNotContain(user.IdRoles, x => x == roleAggregate.IdIdentityServer);

        this.IdentityServerMock.Verify(m => m.RemoveUserFromGroupAsync(userModel.Id, groupModel.Id, It.IsAny<CancellationToken>()), Times.Once);

    }

}

