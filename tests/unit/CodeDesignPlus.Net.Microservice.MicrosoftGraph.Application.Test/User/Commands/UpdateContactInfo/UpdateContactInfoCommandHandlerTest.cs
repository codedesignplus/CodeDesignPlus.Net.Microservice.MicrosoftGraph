using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateContactInfo;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.UpdateContactInfo;

public class UpdateContactInfoCommandHandlerTest
{
    private readonly Mock<IUserRepository> repositoryMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<IIdentityServer> identityServerMock;
    private readonly UpdateContactInfoCommandHandler handler;

    public UpdateContactInfoCommandHandlerTest()
    {
        repositoryMock = new Mock<IUserRepository>();
        mapperMock = new Mock<IMapper>();
        identityServerMock = new Mock<IIdentityServer>();
        handler = new UpdateContactInfoCommandHandler(repositoryMock.Object, mapperMock.Object, identityServerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestError()
    {
        // Arrange
        UpdateContactInfoCommand request = null!;
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, cancellationToken));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUserNotFoundError()
    {
        // Arrange
        var request = new UpdateContactInfoCommand(Guid.NewGuid(), Domain.ValueObjects.ContactInfo.Create("Stree 123", "City", "State", "12345", "Country", "3105631234", ["fake@fake.com"]));
        var cancellationToken = CancellationToken.None;

        repositoryMock
            .Setup(r => r.FindAsync<UserAggregate>(request.Id, cancellationToken))
            .ReturnsAsync((UserAggregate)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, cancellationToken));

        Assert.Equal(Errors.UserNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotExistInIdentityServer_ThrowsUserNotExistInIdentityServerError()
    {
        // Arrange
        var request = new UpdateContactInfoCommand(Guid.NewGuid(), Domain.ValueObjects.ContactInfo.Create("Stree 123", "City", "State", "12345", "Country", "3105631234", ["fake@fake.com"]));
        var user = UserAggregate.Create(request.Id);
        var cancellationToken = CancellationToken.None;

        repositoryMock
            .Setup(r => r.FindAsync<UserAggregate>(request.Id, cancellationToken))
            .ReturnsAsync(user);

        identityServerMock
            .Setup(i => i.GetUserByIdAsync(user.Id, cancellationToken))
            .ReturnsAsync((Domain.Models.User)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, cancellationToken));

        Assert.Equal(Errors.UserNotExistInIdentityServer.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotExistInIdentityServer.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesContactInfo()
    {
        // Arrange
        var request = new UpdateContactInfoCommand(Guid.NewGuid(), Domain.ValueObjects.ContactInfo.Create("Stree 123", "City", "State", "12345", "Country", "3105631234", ["fake@fake.com"]));
        var user = UserAggregate.Create(request.Id);
        var cancellationToken = CancellationToken.None;

        var userModel = new Domain.Models.User() { Id = user.Id };
        var contactInfo = new Domain.Models.ContactInfo
        {
            Address = "Stree 123",
            City = "City",
            State = "State",
            PostalCode = "12345",
            Country = "Country",
            Phone = "3105631234",
            Email = ["fake@info.com"]
        };

        repositoryMock
            .Setup(r => r.FindAsync<UserAggregate>(request.Id, cancellationToken))
            .ReturnsAsync(user);

        identityServerMock
            .Setup(i => i.GetUserByIdAsync(user.Id, cancellationToken))
            .ReturnsAsync(userModel);

        mapperMock
            .Setup(m => m.Map<Domain.Models.ContactInfo>(request.Contact))
            .Returns(contactInfo);

        // Act
        await handler.Handle(request, cancellationToken);

        // Assert
        identityServerMock.Verify(i => i.UpdateContactInfoAsync(user.Id, contactInfo, cancellationToken), Times.Once);
    }
}
