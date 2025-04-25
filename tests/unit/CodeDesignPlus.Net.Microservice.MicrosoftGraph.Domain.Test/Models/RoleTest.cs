using System;
using Xunit;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Test.Models;

public class RoleTest
{
    [Fact]
    public void Role_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Admin";
        var description = "Administrator role";
        var isActive = true;

        // Act
        var role = new Role
        {
            Id = id,
            Name = name,
            Description = description,
            IsActive = isActive
        };

        // Assert
        Assert.Equal(id, role.Id);
        Assert.Equal(name, role.Name);
        Assert.Equal(description, role.Description);
        Assert.Equal(isActive, role.IsActive);
    }
}
