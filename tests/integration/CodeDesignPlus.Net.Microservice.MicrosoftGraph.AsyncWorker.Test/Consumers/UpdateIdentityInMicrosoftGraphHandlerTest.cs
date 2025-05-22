using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Roles;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Helpers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using Moq;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;


public class UpdateIdentityInMicrosoftGraphHandler(Server<Program> server) : ConsumerServerBase(server)
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
        var pubsub = this.Services.GetRequiredService<IPubSub>();
        var userAggregate = UserAggregate.Create(Guid.NewGuid(), "joe@fake.com");
        var domainEvent = new UserUpdatedDomainEvent(userAggregate.Id, "Joe New", "Doe New", "joe-new@fake.com", "3105631234", "Joe New Doe New", false);

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

        this.IdentityServerMock
            .Setup(x => x.GetUserByIdAsync(userModel.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userModel);

        this.IdentityServerMock
            .Setup(x => x.UpdateUserAsync(userModel.Id, It.IsAny<Domain.Models.User>(), It.IsAny<CancellationToken>()));

        await userRepository.CreateAsync(userAggregate, CancellationToken.None);

        // Act
        _ = pubsub.PublishAsync(domainEvent, CancellationToken.None);

        // Assert
        await Task.Delay(TimeSpan.FromSeconds(5));

        this.IdentityServerMock.Verify(m => m.UpdateUserAsync(
            userModel.Id, 
            It.Is<Domain.Models.User>(x =>
                x.FirstName == domainEvent.FirstName &&
                x.LastName == domainEvent.LastName &&
                x.DisplayName == domainEvent.DisplayName &&
                x.Email == domainEvent.Email &&
                x.Phone == domainEvent.Phone &&
                x.IsActive == domainEvent.IsActive
            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

