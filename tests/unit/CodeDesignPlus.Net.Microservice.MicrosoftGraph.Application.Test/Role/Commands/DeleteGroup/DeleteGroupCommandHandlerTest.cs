using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.DeleteGroup;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.Role.Commands.DeleteGroup;

public class DeleteGroupCommandHandlerTest
{
    private readonly Mock<IRoleRepository> repositoryMock;
    private readonly Mock<IIdentityServer> identityServerMock;
    private readonly DeleteGroupCommandHandler handler;

    public DeleteGroupCommandHandlerTest()
    {
        repositoryMock = new Mock<IRoleRepository>();
        identityServerMock = new Mock<IIdentityServer>();
        handler = new DeleteGroupCommandHandler(repositoryMock.Object, identityServerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestError()
    {
        // Arrange
        DeleteGroupCommand request = null!;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_RoleNotFound_ThrowsRoleNotFoundError()
    {
        // Arrange
        var request = new DeleteGroupCommand(Guid.NewGuid());
        repositoryMock
            .Setup(r => r.FindAsync<RoleAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoleAggregate)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.RoleNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.RoleNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_GroupNotFoundInIdentityServer_ThrowsGroupNotFoundError()
    {
        // Arrange
        var request = new DeleteGroupCommand(Guid.NewGuid());
        var roleAggregate = RoleAggregate.Create(request.Id, Guid.NewGuid(), "TestGroup", "Test Description", true);

        repositoryMock
            .Setup(r => r.FindAsync<RoleAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roleAggregate);

        identityServerMock
            .Setup(i => i.GetGroupByIdAsync(roleAggregate.IdIdentityServer, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Models.Role)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.GroupNotFoundInIdentityServer.GetMessage(), exception.Message);
        Assert.Equal(Errors.GroupNotFoundInIdentityServer.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesGroupAndRole()
    {
        // Arrange
        var request = new DeleteGroupCommand(Guid.NewGuid());
        var roleAggregate = RoleAggregate.Create(request.Id, Guid.NewGuid(), "TestGroup", "Test Description", true);

        repositoryMock
            .Setup(r => r.FindAsync<RoleAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roleAggregate);
        identityServerMock
            .Setup(i => i.GetGroupByIdAsync(roleAggregate.IdIdentityServer, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Models.Role() { Id = roleAggregate.IdIdentityServer });
        identityServerMock
            .Setup(i => i.DeleteGroupAsync(roleAggregate.IdIdentityServer, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repositoryMock
            .Setup(r => r.DeleteAsync<RoleAggregate>(roleAggregate.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        identityServerMock.Verify(i => i.DeleteGroupAsync(roleAggregate.IdIdentityServer, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(r => r.DeleteAsync<RoleAggregate>(roleAggregate.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
