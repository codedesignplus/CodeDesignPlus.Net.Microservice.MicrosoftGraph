using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.AddGroupToUser;

public class AddGroupToUserCommandHandlerTest
{
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IRoleRepository> roleRepositoryMock;
    private readonly Mock<IIdentityServer> identityServerMock;
    private readonly AddGroupToUserCommandHandler handler;

    public AddGroupToUserCommandHandlerTest()
    {
        userRepositoryMock = new Mock<IUserRepository>();
        roleRepositoryMock = new Mock<IRoleRepository>();
        identityServerMock = new Mock<IIdentityServer>();
        handler = new AddGroupToUserCommandHandler(userRepositoryMock.Object, roleRepositoryMock.Object, identityServerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        AddGroupToUserCommand request = null!;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        var request = new AddGroupToUserCommand(Guid.NewGuid(), "TestRole");
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
    public async Task Handle_UserNotExistInIdentityServer_ThrowsUserNotExistInIdentityServerException()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid());
        var request = new AddGroupToUserCommand(user.Id, "TestRole");

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
    public async Task Handle_RoleExists_AddsUserToGroupAndUpdatesUser()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid());
        var role = RoleAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), "TestRole", "Test Description", true);
        var request = new AddGroupToUserCommand(user.Id, role.Name);
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
        identityServerMock.Verify(server => server.AddUserToGroupAsync(user.Id, role.IdIdentityServer, It.IsAny<CancellationToken>()), Times.Once);
        userRepositoryMock.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RoleDoesNotExist_AddsUserToGroupAndUpdatesUser()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid());
        var group = RoleAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), "TestRole", "Test Description", true);
        var request = new AddGroupToUserCommand(user.Id, group.Name);
        var userModel = new Domain.Models.User { Id = user.Id };
        var roleModel = new Domain.Models.Role { Id = group.IdIdentityServer };

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
            .ReturnsAsync(roleModel);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identityServerMock.Verify(server => server.AddUserToGroupAsync(user.Id, group.IdIdentityServer, It.IsAny<CancellationToken>()), Times.Once);
        userRepositoryMock.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
