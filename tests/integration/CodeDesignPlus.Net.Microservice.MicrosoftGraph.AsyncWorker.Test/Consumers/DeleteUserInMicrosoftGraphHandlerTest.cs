using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Helpers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using Moq;


namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class DeleteUserInMicrosoftGraphHandlerTest(Server<Program> server) : ConsumerServerBase(server)
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
        var userAggregate = UserAggregate.Create(Guid.NewGuid());
        var domainEvent = new UserDeletedDomainEvent(userAggregate.Id, "Joe", "Doe", "joe@fake.com", "3105631234", "Joe Doe", true);

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
            .Setup(x => x.DeleteUserAsync(userModel.Id, It.IsAny<CancellationToken>()));

        await userRepository.CreateAsync(userAggregate, CancellationToken.None);

        // Act
        _ = pubsub.PublishAsync(domainEvent, CancellationToken.None);

        // Assert
        var user = await Retry(async () =>
        {
            var item = await userRepository.FindAsync<UserAggregate>(domainEvent.AggregateId, CancellationToken.None);

            if (item != null)
                return null;

            return item;
        });

        Assert.Null(user);

        this.IdentityServerMock.Verify(m => m.DeleteUserAsync(userModel.Id, It.IsAny<CancellationToken>()), Times.Once);

    }

}

