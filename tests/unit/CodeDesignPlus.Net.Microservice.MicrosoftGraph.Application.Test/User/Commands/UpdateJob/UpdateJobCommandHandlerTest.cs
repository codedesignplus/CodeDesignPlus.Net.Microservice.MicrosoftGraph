using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateJob;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.UpdateJob;

public class UpdateJobCommandHandlerTest
{
    private readonly Mock<IUserRepository> repositoryMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<IIdentityServer> identityServerMock;
    private readonly UpdateJobCommandHandler handler;

    public UpdateJobCommandHandlerTest()
    {
        repositoryMock = new Mock<IUserRepository>();
        mapperMock = new Mock<IMapper>();
        identityServerMock = new Mock<IIdentityServer>();
        handler = new UpdateJobCommandHandler(repositoryMock.Object, mapperMock.Object, identityServerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestError()
    {
        // Arrange
        UpdateJobCommand request = null!;

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
        var request = new UpdateJobCommand(Guid.NewGuid(), Domain.ValueObjects.JobInfo.Create("Software Engineer", "Tech Company", "It", "123456", "Full Time", SystemClock.Instance.GetCurrentInstant(), "Remote"));
        repositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
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
        var user = UserAggregate.Create(Guid.NewGuid());
        var request = new UpdateJobCommand(user.Id, Domain.ValueObjects.JobInfo.Create("Software Engineer", "Tech Company", "It", "123456", "Full Time", SystemClock.Instance.GetCurrentInstant(), "Remote"));

        repositoryMock
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
    public async Task Handle_ValidRequest_UpdatesJobInfo()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid());
        var request = new UpdateJobCommand(user.Id, Domain.ValueObjects.JobInfo.Create("Software Engineer", "Tech Company", "It", "123456", "Full Time", SystemClock.Instance.GetCurrentInstant(), "Remote"));
        var userModel = new Domain.Models.User { Id = user.Id };
        var jobModel = new Domain.Models.JobInfo { 
            CompanyName = "Tech Company New",
            Department = "It New",
            JobTitle = "Software Engineer New",
            EmployeeId = "123456 New",
            EmployeeType = "Full Time New",
            EmployHireDate = SystemClock.Instance.GetCurrentInstant(),
            OfficeLocation = "Remote New",
         };

        repositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        identityServerMock
            .Setup(server => server.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userModel);
        mapperMock
            .Setup(mapper => mapper.Map<Domain.Models.JobInfo>(request.Job))
            .Returns(jobModel);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identityServerMock.Verify(server => server.UpdateJobInfoAsync(user.Id, It.IsAny<Domain.Models.JobInfo>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
