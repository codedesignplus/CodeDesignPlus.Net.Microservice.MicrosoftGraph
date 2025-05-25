using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.UpdateIdentity;

public class UpdateIdentityCommandHandlerTest
{
    private readonly Mock<IUserRepository> repositoryMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<IIdentityServer> identityServerMock;
    private readonly UpdateIdentityCommandHandler handler;

    public UpdateIdentityCommandHandlerTest()
    {
        repositoryMock = new Mock<IUserRepository>();
        mapperMock = new Mock<IMapper>();
        identityServerMock = new Mock<IIdentityServer>();
        handler = new UpdateIdentityCommandHandler(repositoryMock.Object, mapperMock.Object, identityServerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        UpdateIdentityCommand request = null!;

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
        var request = new UpdateIdentityCommand(Guid.NewGuid(), "Joe", "Doe", "Joe Doe", "joe.doe@fake.com", "3105631234", true);
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
    public async Task Handle_UserNotExistInIdentityServer_ThrowsUserNotExistInIdentityServerException()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid(), "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "123456", true);
        var request = new UpdateIdentityCommand(Guid.NewGuid(), "Joe", "Doe", "Joe Doe", "joe.doe@fake.com", "3105631234", true);

        repositoryMock
            .Setup(r => r.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        identityServerMock
            .Setup(i => i.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Models.User)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.UserNotExistInIdentityServer.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotExistInIdentityServer.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesUserSuccessfully()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid(), "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "123456", true);
        var request = new UpdateIdentityCommand(Guid.NewGuid(), "Joe", "Doe", "Joe Doe", "joe.doe@fake.com", "3105631234", true);
        var mappedUser = new Domain.Models.User();

        repositoryMock
            .Setup(r => r.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        identityServerMock
            .Setup(i => i.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Models.User());
        mapperMock
            .Setup(m => m.Map<Domain.Models.User>(request))
            .Returns(mappedUser);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identityServerMock.Verify(i => i.UpdateUserAsync(user.Id, mappedUser, It.IsAny<CancellationToken>()), Times.Once);
    }
}
