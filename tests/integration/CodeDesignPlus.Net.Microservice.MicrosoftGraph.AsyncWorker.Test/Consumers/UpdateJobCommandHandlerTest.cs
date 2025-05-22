using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Helpers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using Microsoft.Kiota.Abstractions;
using Moq;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class UpdateJobCommandHandlerTest(Server<Program> server) : ConsumerServerBase(server)
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
        var userAggregate = UserAggregate.Create(Guid.NewGuid(), "joee.doenew@fake.com");
        var domainEvent = new JobInfoUpdatedDomainEvent(userAggregate.Id, Domain.ValueObjects.JobInfo.Create(
            "Software Engineer New",
            "Tech Company New",
            "It New",
            "123456 New",
            "Full Time New",
            SystemClock.Instance.GetCurrentInstant(),
            "Remote New"
        ));

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
            .Setup(x => x.UpdateJobInfoAsync(userModel.Id, It.IsAny<Domain.Models.JobInfo>(), It.IsAny<CancellationToken>()));

        await userRepository.CreateAsync(userAggregate, CancellationToken.None);

        // Act
        _ = pubsub.PublishAsync(domainEvent, CancellationToken.None);

        // Assert
        await Task.Delay(TimeSpan.FromSeconds(5));

        this.IdentityServerMock.Verify(m => m.UpdateJobInfoAsync(
            userModel.Id, 
            It.Is<Domain.Models.JobInfo>(x =>
                x.EmployeeId == domainEvent.Job.EmployeeId &&
                x.EmployeeType == domainEvent.Job.EmployeeType &&
                x.CompanyName == domainEvent.Job.CompanyName &&
                x.JobTitle == domainEvent.Job.JobTitle &&
                x.Department == domainEvent.Job.Department &&
                x.OfficeLocation == domainEvent.Job.OfficeLocation &&
                x.EmployHireDate.ToString() == domainEvent.Job.EmployHireDate.ToString()
            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }

}

