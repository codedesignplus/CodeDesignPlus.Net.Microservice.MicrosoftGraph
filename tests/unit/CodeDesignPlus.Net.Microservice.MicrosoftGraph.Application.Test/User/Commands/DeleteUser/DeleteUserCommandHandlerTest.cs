using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.DeleteUser;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.DeleteUser;

public class DeleteUserCommandHandlerTest
{
    private readonly Mock<IUserRepository> repositoryMock;
    private readonly Mock<IIdentityServer> identityServerMock;
    private readonly DeleteUserCommandHandler handler;

    public DeleteUserCommandHandlerTest()
    {
        repositoryMock = new Mock<IUserRepository>();
        identityServerMock = new Mock<IIdentityServer>();
        handler = new DeleteUserCommandHandler(repositoryMock.Object, identityServerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestError()
    {
        // Arrange
        DeleteUserCommand request = null!;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotFoundInRepository_ThrowsRoleNotFoundError()
    {
        // Arrange
        var request = new DeleteUserCommand(Guid.NewGuid());
        repositoryMock
            .Setup(r => r.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAggregate)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.UserNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotFoundInIdentityServer_ThrowsUserNotExistInIdentityServerError()
    {
        // Arrange
        var request = new DeleteUserCommand(Guid.NewGuid());
        var userAggregate = UserAggregate.Create(request.Id, "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", false, true);

        repositoryMock
            .Setup(r => r.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userAggregate);
        identityServerMock
            .Setup(i => i.GetUserByIdAsync(userAggregate.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Models.User)null!);

        // Act & Assert
        var exception =  await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.UserNotExistInIdentityServer.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotExistInIdentityServer.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesUserSuccessfully()
    {
        // Arrange
        var request = new DeleteUserCommand(Guid.NewGuid());
        var userAggregate = UserAggregate.Create(request.Id, "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", false, true);

        var userModel = new Domain.Models.User() { Id = userAggregate.Id };

        repositoryMock
            .Setup(r => r.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userAggregate);
        identityServerMock
            .Setup(i => i.GetUserByIdAsync(userAggregate.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userModel);
        identityServerMock
            .Setup(i => i.DeleteUserAsync(userAggregate.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repositoryMock
            .Setup(r => r.DeleteAsync<UserAggregate>(userAggregate.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identityServerMock.Verify(i => i.DeleteUserAsync(userAggregate.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(r => r.DeleteAsync<UserAggregate>(userAggregate.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
