using System;
using Xunit;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Test.Models;

public class UserTest
{
    [Fact]
    public void User_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var displayName = "John Doe";
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phoneNumber = "123-456-7890";
        var job = new JobInfo();
        var contact = new ContactInfo();
        var isActive = true;

        // Act
        var user = new User
        {
            Id = id,
            DisplayName = displayName,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phoneNumber,
            Job = job,
            Contact = contact,
            IsActive = isActive
        };

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(displayName, user.DisplayName);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(email, user.Email);
        Assert.Equal(phoneNumber, user.Phone);
        Assert.Equal(job, user.Job);
        Assert.Equal(contact, user.Contact);
        Assert.Equal(isActive, user.IsActive);
    }

    [Fact]
    public void ContactInfo_ShouldInitializeWithDefaultValues()
    {
        // Act
        var contactInfo = new ContactInfo();

        // Assert
        Assert.Null(contactInfo.Address);
        Assert.Null(contactInfo.City);
        Assert.Null(contactInfo.State);
        Assert.Null(contactInfo.Country);
        Assert.Null(contactInfo.ZipCode);
        Assert.Null(contactInfo.Phone);
        Assert.Empty(contactInfo.Email);
    }

    [Fact]
    public void JobInfo_ShouldInitializeWithDefaultValues()
    {
        // Act
        var jobInfo = new JobInfo();

        // Assert
        Assert.Null(jobInfo.JobTitle);
        Assert.Null(jobInfo.CompanyName);
        Assert.Null(jobInfo.Department);
        Assert.Null(jobInfo.EmployeeId);
        Assert.Null(jobInfo.EmployeeType);
        Assert.Null(jobInfo.EmployHireDate);
        Assert.Null(jobInfo.OfficeLocation);
    }
}
