using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.RemoveGroupToUser;

public class RemoveGroupToUserCommandHandlerTest
{
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IRoleRepository> roleRepositoryMock;
    private readonly Mock<IIdentityServer> identityServerMock;
    private readonly RemoveGroupToUserCommandHandler handler;

    public RemoveGroupToUserCommandHandlerTest()
    {
        userRepositoryMock = new Mock<IUserRepository>();
        roleRepositoryMock = new Mock<IRoleRepository>();
        identityServerMock = new Mock<IIdentityServer>();
        handler = new RemoveGroupToUserCommandHandler(userRepositoryMock.Object, roleRepositoryMock.Object, identityServerMock.Object);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsException()
    {
        // Arrange
        RemoveGroupToUserCommand request = null!;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsException()
    {
        // Arrange
        var request = new RemoveGroupToUserCommand(Guid.NewGuid(), "TestRole");
        userRepositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAggregate)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.UserNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotExistInIdentityServer_ThrowsException()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid(), "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", false, true);
        var request = new RemoveGroupToUserCommand(user.Id, "TestRole");

        userRepositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        identityServerMock
            .Setup(server => server.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Models.User)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.UserNotExistInIdentityServer.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotExistInIdentityServer.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_RemovesUserFromGroupAndUpdatesUser()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid(), "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", false, true);
        var role = RoleAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), "TestRole", "Test Description", true);
        user.AddRole(role.IdIdentityServer);

        var request = new RemoveGroupToUserCommand(user.Id, role.Name);
        var userModel = new Domain.Models.User { Id = user.Id };

        userRepositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        identityServerMock
            .Setup(server => server.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userModel);
        roleRepositoryMock
            .Setup(repo => repo.GetByNameAsync(request.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identityServerMock.Verify(server => server.RemoveUserFromGroupAsync(user.Id, role.IdIdentityServer, It.IsAny<CancellationToken>()), Times.Once);
        userRepositoryMock.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_GroupNotFoundInRole_UsesIdentityServerGroup()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid(), "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", false, true);
        var role = RoleAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), "TestRole", "Test Description", true);
        user.AddRole(role.IdIdentityServer);
        var request = new RemoveGroupToUserCommand(user.Id, role.Name);
        var userModel = new Domain.Models.User { Id = user.Id };
        var groupModel = new Domain.Models.Role { Id = role.IdIdentityServer };

        userRepositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        identityServerMock
            .Setup(server => server.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userModel);
        roleRepositoryMock
            .Setup(repo => repo.GetByNameAsync(request.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoleAggregate)null!);
        identityServerMock
            .Setup(server => server.GetGroupByNameAsync(request.Role, It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupModel);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identityServerMock.Verify(server => server.RemoveUserFromGroupAsync(user.Id, role.IdIdentityServer, It.IsAny<CancellationToken>()), Times.Once);
        userRepositoryMock.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
