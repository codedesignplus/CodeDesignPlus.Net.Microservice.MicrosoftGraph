using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.UpdateProfile;

public class UpdateProfileCommandHandlerTest
{
    private readonly Mock<IUserRepository> repositoryMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<IIdentityServer> identityServerMock;
    private readonly UpdateProfileCommandHandler handler;

    public UpdateProfileCommandHandlerTest()
    {
        repositoryMock = new Mock<IUserRepository>();
        mapperMock = new Mock<IMapper>();
        identityServerMock = new Mock<IIdentityServer>();
        handler = new UpdateProfileCommandHandler(repositoryMock.Object, mapperMock.Object, identityServerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestError()
    {
        // Arrange
        UpdateProfileCommand request = null!;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUserNotFoundError()
    {
        // Arrange
        var contactInfo = Domain.ValueObjects.ContactInfo.Create("Street 123", "City", "State", "12345", "Country", "3105631234", ["joee.doe@fake.com"]);
        var jobInfo = Domain.ValueObjects.JobInfo.Create("Software Engineer", "Tech Company", "It", "123456", "Full Time", SystemClock.Instance.GetCurrentInstant(), "Remote");
        var request = new UpdateProfileCommand(Guid.NewGuid(), "Joe", "Doe", "Joe Doe", "joe.doe@fake.com", "3105631234", contactInfo, jobInfo, true);

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
    public async Task Handle_UserNotExistInIdentityServer_ThrowsUserNotExistInIdentityServerError()
    {
        // Arrange
        var contactInfo = Domain.ValueObjects.ContactInfo.Create("Street 123", "City", "State", "12345", "Country", "3105631234", ["joee.doe@fake.com"]);
        var jobInfo = Domain.ValueObjects.JobInfo.Create("Software Engineer", "Tech Company", "It", "123456", "Full Time", SystemClock.Instance.GetCurrentInstant(), "Remote");
        var request = new UpdateProfileCommand(Guid.NewGuid(), "Joe", "Doe", "Joe Doe", "joe.doe@fake.com", "3105631234", contactInfo, jobInfo, true);

        var user = UserAggregate.Create(request.Id, Guid.NewGuid(), Domain.Enums.IdentityProvider.MicrosoftEntraExternalId, "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", false, true);
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
        var contactInfo = Domain.ValueObjects.ContactInfo.Create("Street 123", "City", "State", "12345", "Country", "3105631234", ["joee.doe@fake.com"]);
        var jobInfo = Domain.ValueObjects.JobInfo.Create("Software Engineer", "Tech Company", "It", "123456", "Full Time", SystemClock.Instance.GetCurrentInstant(), "Remote");
        var request = new UpdateProfileCommand(Guid.NewGuid(), "Joe", "Doe", "Joe Doe", "joe.doe@fake.com", "3105631234", contactInfo, jobInfo, true);

        var user = UserAggregate.Create(request.Id, Guid.NewGuid(), Domain.Enums.IdentityProvider.MicrosoftEntraExternalId, "Joe", "Doe", "joee.doenew@fake.com", "3107545252", "Joe Doe", "key", "cipher", false, true);
        var userModel = new Domain.Models.User();

        repositoryMock
            .Setup(r => r.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        identityServerMock
            .Setup(i => i.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userModel);
        mapperMock
            .Setup(m => m.Map<Domain.Models.User>(request))
            .Returns(userModel);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identityServerMock.Verify(i => i.UpdateUserAsync(user.Id, userModel, It.IsAny<CancellationToken>()), Times.Once);
        identityServerMock.Verify(i => i.UpdateContactInfoAsync(user.Id, userModel.Contact, It.IsAny<CancellationToken>()), Times.Once);
        identityServerMock.Verify(i => i.UpdateJobInfoAsync(user.Id, userModel.Job, It.IsAny<CancellationToken>()), Times.Once);
    }
}
