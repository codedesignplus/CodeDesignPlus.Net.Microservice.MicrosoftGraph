using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.CreateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.Role.Commands.CreateGroup;

public class CreateGroupCommandHandlerTest
{
    private readonly Mock<IRoleRepository> repositoryMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<IIdentityServer> identityServerMock;
    private readonly CreateGroupCommandHandler handler;

    public CreateGroupCommandHandlerTest()
    {
        repositoryMock = new Mock<IRoleRepository>();
        mapperMock = new Mock<IMapper>();
        identityServerMock = new Mock<IIdentityServer>();
        handler = new CreateGroupCommandHandler(repositoryMock.Object, mapperMock.Object, identityServerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestError()
    {
        // Arrange
        CreateGroupCommand request = null!;
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, cancellationToken));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_RoleAlreadyExists_ThrowsRoleAlreadyExistsError()
    {
        // Arrange
        var request = new CreateGroupCommand(Guid.NewGuid(), "TestGroup", "Test Description", true);
        var cancellationToken = CancellationToken.None;

        repositoryMock.Setup(r => r.ExistsAsync<RoleAggregate>(request.Id, cancellationToken))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, cancellationToken));

        Assert.Equal(Errors.RoleAlreadyExists.GetMessage(), exception.Message);
        Assert.Equal(Errors.RoleAlreadyExists.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_GroupAlreadyExistsInIdentityServer_ThrowsGroupAlreadyExistsError()
    {
        // Arrange
        var request = new CreateGroupCommand(Guid.NewGuid(), "TestGroup", "Test Description", true);
        var group = new Domain.Models.Role() { Id = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;

        repositoryMock
            .Setup(r => r.ExistsAsync<RoleAggregate>(request.Id, cancellationToken))
            .ReturnsAsync(false);

        identityServerMock
            .Setup(i => i.GetGroupByNameAsync(request.Name, cancellationToken))
            .ReturnsAsync(group);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, cancellationToken));

        Assert.Equal(Errors.GroupAlreadyExistsInIdentityServer.GetMessage(), exception.Message);
        Assert.Equal(Errors.GroupAlreadyExistsInIdentityServer.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesGroupAndRole()
    {
        // Arrange
        var request = new CreateGroupCommand(Guid.NewGuid(), "TestGroup", "Test Description", true);

        var cancellationToken = CancellationToken.None;

        repositoryMock.Setup(r => r.ExistsAsync<RoleAggregate>(request.Id, cancellationToken))
            .ReturnsAsync(false);

        identityServerMock.Setup(i => i.GetGroupByNameAsync(request.Name, cancellationToken))
            .ReturnsAsync((Domain.Models.Role)null!);

        var group = new Domain.Models.Role() { Id = Guid.NewGuid() };
        identityServerMock.Setup(i => i.CreateGroupAsync(It.IsAny<Domain.Models.Role>(), cancellationToken))
            .ReturnsAsync(group);

        mapperMock.Setup(m => m.Map<Domain.Models.Role>(request))
            .Returns(new Domain.Models.Role());

        // Act
        await handler.Handle(request, cancellationToken);

        // Assert
        repositoryMock.Verify(r => r.CreateAsync(It.IsAny<RoleAggregate>(), cancellationToken), Times.Once);
    }
}
