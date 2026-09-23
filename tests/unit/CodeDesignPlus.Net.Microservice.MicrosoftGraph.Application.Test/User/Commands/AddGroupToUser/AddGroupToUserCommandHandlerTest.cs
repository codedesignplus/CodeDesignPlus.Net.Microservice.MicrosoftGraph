using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;
using CodeDesignPlus.Net.Cache.Abstractions;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.AddGroupToUser;

public class AddGroupToUserCommandHandlerTest
{
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IIdentityServer> identityServerMock;
    private readonly AddGroupToUserCommandHandler handler;

    public AddGroupToUserCommandHandlerTest()
    {
        userRepositoryMock = new Mock<IUserRepository>();
        identityServerMock = new Mock<IIdentityServer>();
        handler = new AddGroupToUserCommandHandler(userRepositoryMock.Object, identityServerMock.Object, Mock.Of<ICacheManager>());
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
        var request = new AddGroupToUserCommand(Guid.NewGuid(), Guid.NewGuid());
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
        var user = UnUsuario();
        var request = new AddGroupToUserCommand(user.Id, Guid.NewGuid());

        userRepositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        identityServerMock
            .Setup(server => server.GetUserByIdAsync(user.IdentityProviderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Models.User)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.UserNotExistInIdentityServer.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotExistInIdentityServer.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_AddsUserToTheGroupItWasGiven()
    {
        // Arrange: el rol llega ya como identificador del grupo. Antes viajaba el nombre y habia que
        // buscarlo en la base local o preguntarselo al proveedor de identidad, con dos caminos distintos
        // segun si estaba replicado o no.
        var user = UnUsuario();
        var grupo = Guid.Parse("1a43656c-f457-4695-8bfd-903be4b66097");
        var request = new AddGroupToUserCommand(user.Id, grupo);

        userRepositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        identityServerMock
            .Setup(server => server.GetUserByIdAsync(user.IdentityProviderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Models.User { Id = user.Id });

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identityServerMock.Verify(server => server.AddUserToGroupAsync(user.IdentityProviderId, grupo, It.IsAny<CancellationToken>()), Times.Once);
        userRepositoryMock.Verify(repo => repo.AddRoleAsync(user.Id, grupo, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static UserAggregate UnUsuario() => UserAggregate.Create(
        Guid.NewGuid(), Guid.NewGuid(), Domain.Enums.IdentityProvider.MicrosoftEntraExternalId,
        "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "1234567890", null, "key", "cipher", false, true);
}
