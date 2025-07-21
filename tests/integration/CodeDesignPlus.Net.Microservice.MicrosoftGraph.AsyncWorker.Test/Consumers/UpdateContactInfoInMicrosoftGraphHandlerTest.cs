using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Helpers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using Microsoft.Kiota.Abstractions;
using Moq;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

public class UpdateContactInfoInMicrosoftGraphHandlerTest(Server<Program> server) : ConsumerServerBase(server)
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
        var userAggregate = UserAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), Domain.Enums.IdentityProvider.MicrosoftEntraExternalId, "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", false, true);
        var domainEvent = new ContactInfoUpdatedDomainEvent(userAggregate.Id, Domain.ValueObjects.ContactInfo.Create(
            "Street 123 New",
            "City New",
            "State New",
            "Country New",
            "12345 New",
            "3105631235",
            ["joee.doenew@fake.com"]
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
            .Setup(x => x.UpdateContactInfoAsync(userModel.Id, It.IsAny<Domain.Models.ContactInfo>(), It.IsAny<CancellationToken>()));

        await userRepository.CreateAsync(userAggregate, CancellationToken.None);

        // Act
        _ = pubsub.PublishAsync(domainEvent, CancellationToken.None);

        // Assert
        await Task.Delay(TimeSpan.FromSeconds(5));

        this.IdentityServerMock.Verify(m => m.UpdateContactInfoAsync(
            userModel.Id, 
            It.Is<Domain.Models.ContactInfo>(x =>
                x.Country == domainEvent.Contact.Country &&
                x.Address == domainEvent.Contact.Address &&
                x.City == domainEvent.Contact.City &&
                x.State == domainEvent.Contact.State &&
                x.ZipCode == domainEvent.Contact.ZipCode &&
                x.Phone == domainEvent.Contact.Phone 
            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }

}

