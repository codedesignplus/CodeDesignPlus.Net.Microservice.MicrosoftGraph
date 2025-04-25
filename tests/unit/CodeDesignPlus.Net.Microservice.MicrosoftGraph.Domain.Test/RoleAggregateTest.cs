using System;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Test;

public class RoleAggregateTest
{
    [Fact]
    public void Create_ValidParameters_ReturnsRoleAggregate()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idIdentityServer = Guid.NewGuid();
        var name = "Admin";
        var description = "Administrator role";
        var isActive = true;

        // Act
        var role = RoleAggregate.Create(id, idIdentityServer, name, description, isActive);

        // Assert
        Assert.NotNull(role);
        Assert.Equal(id, role.Id);
        Assert.Equal(idIdentityServer, role.IdIdentityServer);
        Assert.Equal(name, role.Name);
        Assert.Equal(description, role.Description);
        Assert.Equal(isActive, role.IsActive);
    }

    [Fact]
    public void Create_InvalidId_ThrowsException()
    {
        // Arrange
        var id = Guid.Empty;
        var idIdentityServer = Guid.NewGuid();
        var name = "Admin";
        var description = "Administrator role";
        var isActive = true;

        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => RoleAggregate.Create(id, idIdentityServer, name, description, isActive));

        Assert.Equal(Errors.IdIsInvalid.GetMessage(), exception.Message);
        Assert.Equal(Errors.IdIsInvalid.GetCode(), exception.Code);
        Assert.Equal(Layer.Domain, exception.Layer);
    }

    [Fact]
    public void Update_ValidParameters_UpdatesProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idIdentityServer = Guid.NewGuid();
        var role = RoleAggregate.Create(id, idIdentityServer, "Admin", "Administrator role", true);

        var newName = "User";
        var newDescription = "User role";
        var isActive = false;

        // Act
        role.Update(newName, newDescription, isActive);

        // Assert
        Assert.Equal(newName, role.Name);
        Assert.Equal(newDescription, role.Description);
        Assert.Equal(isActive, role.IsActive);
    }

    [Fact]
    public void Update_InvalidName_ThrowsException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idIdentityServer = Guid.NewGuid();
        var role = RoleAggregate.Create(id, idIdentityServer, "Admin", "Administrator role", true);

        var newName = string.Empty;
        var newDescription = "User role";
        var isActive = false;

        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => role.Update(newName, newDescription, isActive));

        Assert.Equal(Errors.NameRequired.GetMessage(), exception.Message);
        Assert.Equal(Errors.NameRequired.GetCode(), exception.Code);
        Assert.Equal(Layer.Domain, exception.Layer);
    }
}
