namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Test;

public class UserAggregateTest
{
    [Fact]
    public void Create_ValidId_ReturnsUserAggregate()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var userAggregate = UserAggregate.Create(id, "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", true);

        // Assert
        Assert.NotNull(userAggregate);
        Assert.Equal(id, userAggregate.Id);
    }

    [Fact]
    public void Create_InvalidId_ThrowsException()
    {
        // Arrange
        var id = Guid.Empty;

        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => UserAggregate.Create(id, "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe",  "key", "cipher", true));

        Assert.Equal(Errors.IdIsInvalid.GetMessage(), exception.Message);
        Assert.Equal(Errors.IdIsInvalid.GetCode(), exception.Code);
        Assert.Equal(Layer.Domain, exception.Layer);
    }

    [Fact]
    public void AddRole_ValidRole_AddsRole()
    {
        // Arrange
        var userAggregate = UserAggregate.Create(Guid.NewGuid(), "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", true);
        var roleId = Guid.NewGuid();

        // Act
        userAggregate.AddRole(roleId);

        // Assert
        Assert.Contains(roleId, userAggregate.IdRoles);
    }

    [Fact]
    public void AddRole_DuplicateRole_ThrowsException()
    {
        // Arrange
        var userAggregate = UserAggregate.Create(Guid.NewGuid(), "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", true);
        var roleId = Guid.NewGuid();
        userAggregate.AddRole(roleId);

        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => userAggregate.AddRole(roleId));

        Assert.Equal(Errors.RoleAlreadyAdded.GetMessage(), exception.Message);
        Assert.Equal(Errors.RoleAlreadyAdded.GetCode(), exception.Code);
        Assert.Equal(Layer.Domain, exception.Layer);
    }

    [Fact]
    public void RemoveRole_ValidRole_RemovesRole()
    {
        // Arrange
        var userAggregate = UserAggregate.Create(Guid.NewGuid(), "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", true);
        var roleId = Guid.NewGuid();
        userAggregate.AddRole(roleId);

        // Act
        userAggregate.RemoveRole(roleId);

        // Assert
        Assert.DoesNotContain(roleId, userAggregate.IdRoles);
    }

    [Fact]
    public void RemoveRole_NonExistentRole_ThrowsException()
    {
        // Arrange
        var userAggregate = UserAggregate.Create(Guid.NewGuid(), "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", true);
        var roleId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => userAggregate.RemoveRole(roleId));

        Assert.Equal(Errors.RoleCannotBeRemoved.GetMessage(), exception.Message);
        Assert.Equal(Errors.RoleCannotBeRemoved.GetCode(), exception.Code);
        Assert.Equal(Layer.Domain, exception.Layer);
    }
}
