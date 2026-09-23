using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;
using CodeDesignPlus.Net.Cache.Abstractions;

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
        handler = new RemoveGroupToUserCommandHandler(userRepositoryMock.Object, roleRepositoryMock.Object, identityServerMock.Object, Mock.Of<ICacheManager>());
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
        var request = new RemoveGroupToUserCommand(Guid.NewGuid(), Guid.NewGuid());
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
        var user = UserAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), Domain.Enums.IdentityProvider.MicrosoftEntraExternalId, "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "1234567890", null, "key", "cipher", false, true);
        var request = new RemoveGroupToUserCommand(user.Id, Guid.NewGuid());

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
    public async Task Handle_TranslatesTheCatalogueRoleIntoItsIdentityProviderGroup()
    {
        // Arrange: el rol viaja con el identificador del catalogo y este micro lo traduce al grupo, que
        // es lo unico que entiende el proveedor de identidad.
        var user = UserAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), Domain.Enums.IdentityProvider.MicrosoftEntraExternalId, "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "1234567890", null, "key", "cipher", false, true);
        var rol = Guid.Parse("20000000-0000-0000-0000-000000000007");
        var grupo = Guid.Parse("d13dc2fd-59ce-4462-ae03-2a830a243c56");
        user.AddRole(grupo);

        var request = new RemoveGroupToUserCommand(user.Id, rol);

        userRepositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        roleRepositoryMock
            .Setup(repo => repo.FindAsync<RoleAggregate>(rol, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RoleAggregate.Create(rol, grupo, "Residente", "Residente", true));
        identityServerMock
            .Setup(server => server.GetUserByIdAsync(user.IdentityProviderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Models.User { Id = user.Id });

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identityServerMock.Verify(server => server.RemoveUserFromGroupAsync(user.IdentityProviderId, grupo, It.IsAny<CancellationToken>()), Times.Once);
        userRepositoryMock.Verify(repo => repo.RemoveRoleAsync(user.Id, grupo, It.IsAny<CancellationToken>()), Times.Once);
    }
}
