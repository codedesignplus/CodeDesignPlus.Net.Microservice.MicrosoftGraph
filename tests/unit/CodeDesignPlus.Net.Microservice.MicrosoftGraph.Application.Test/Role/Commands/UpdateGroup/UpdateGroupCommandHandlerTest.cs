using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.Role.Commands.UpdateGroup;

public class UpdateGroupCommandHandlerTest
{
    private readonly Mock<IRoleRepository> repositoryMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<IIdentityServer> identityServerMock;
    private readonly UpdateGroupCommandHandler handler;

    public UpdateGroupCommandHandlerTest()
    {
        repositoryMock = new Mock<IRoleRepository>();
        mapperMock = new Mock<IMapper>();
        identityServerMock = new Mock<IIdentityServer>();
        handler = new UpdateGroupCommandHandler(repositoryMock.Object, mapperMock.Object, identityServerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        UpdateGroupCommand request = null!;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_RoleNotFound_ThrowsRoleNotFoundException()
    {
        // Arrange
        var request = new UpdateGroupCommand(Guid.NewGuid(), "TestGroup", "Test Description", true);
        repositoryMock
            .Setup(repo => repo.FindAsync<RoleAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoleAggregate)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.RoleNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.RoleNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_GroupNotFoundInIdentityServer_ThrowsGroupNotFoundException()
    {
        // Arrange
        var role = RoleAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), "TestGroup", "Test Description", true);
        var request = new UpdateGroupCommand(role.Id, "New Name", "New Description", true);
        repositoryMock
            .Setup(repo => repo.FindAsync<RoleAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        identityServerMock
            .Setup(server => server.GetGroupByIdAsync(role.IdIdentityServer, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Models.Role)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.GroupNotFoundInIdentityServer.GetMessage(), exception.Message);
        Assert.Equal(Errors.GroupNotFoundInIdentityServer.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesGroupAndRole()
    {
        // Arrange
        var role = RoleAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), "TestGroup", "Test Description", true);
        var request = new UpdateGroupCommand(role.Id, "New Name", "New Description", true);
        var group = new Domain.Models.Role() { Id = role.IdIdentityServer };
        var mappedRole = new Domain.Models.Role();

        repositoryMock
            .Setup(repo => repo.FindAsync<RoleAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        identityServerMock
            .Setup(server => server.GetGroupByIdAsync(role.IdIdentityServer, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        mapperMock
            .Setup(mapper => mapper.Map<Domain.Models.Role>(request))
            .Returns(mappedRole);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identityServerMock.Verify(server => server.UpdateGroupAsync(role.IdIdentityServer, mappedRole, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repo => repo.UpdateAsync(role, It.IsAny<CancellationToken>()), Times.Once);
    }
}
