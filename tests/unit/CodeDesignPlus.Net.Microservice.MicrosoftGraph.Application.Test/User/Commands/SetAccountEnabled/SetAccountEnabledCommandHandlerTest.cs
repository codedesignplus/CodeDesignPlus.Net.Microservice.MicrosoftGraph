using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.SetAccountEnabled;
using CodeDesignPlus.Net.Cache.Abstractions;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.SetAccountEnabled;

/// <summary>
/// La cuenta se habilita o deshabilita en el proveedor de identidad con su id de allí, no con el de ms-users.
/// </summary>
public class SetAccountEnabledCommandHandlerTest
{
    [Fact]
    public async Task Handle_Deactivated_DisablesTheAccountInTheIdentityProvider()
    {
        // Arrange
        var identityProviderId = Guid.NewGuid();
        var user = UserAggregate.Create(Guid.NewGuid(), identityProviderId, Domain.Enums.IdentityProvider.MicrosoftEntraExternalId, "Laura", "Gómez", "laura@fake.com", "3105550131", "Laura Gómez", "1023456781", new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC"), "key", "cipher", false, true);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.FindAsync<UserAggregate>(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var identityServer = new Mock<IIdentityServer>();
        var handler = new SetAccountEnabledCommandHandler(repository.Object, identityServer.Object, Mock.Of<ICacheManager>());

        // Act
        await handler.Handle(new SetAccountEnabledCommand(user.Id, false), CancellationToken.None);

        // Assert
        identityServer.Verify(x => x.SetAccountEnabledAsync(identityProviderId, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_Throws()
    {
        // Arrange
        var handler = new SetAccountEnabledCommandHandler(Mock.Of<IUserRepository>(), Mock.Of<IIdentityServer>(), Mock.Of<ICacheManager>());

        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(new SetAccountEnabledCommand(Guid.NewGuid(), true), CancellationToken.None));

        // Assert
        Assert.Equal(Errors.UserNotFound.GetCode(), exception.Code);
    }
}
