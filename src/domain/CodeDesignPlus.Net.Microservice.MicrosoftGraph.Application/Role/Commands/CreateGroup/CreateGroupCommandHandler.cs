using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.CreateGroup;

public class CreateGroupCommandHandler(IRoleRepository repository, IMapper mapper, IIdentityServer identityServer) : IRequestHandler<CreateGroupCommand>
{
    public async Task Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync<RoleAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.RoleAlreadyExists);

        var groupExist = await identityServer.GetGroupByNameAsync(request.Name, cancellationToken);

        ApplicationGuard.IsNotNull(groupExist, Errors.GroupAlreadyExistsInIdentityServer);

        var group = await identityServer.CreateGroupAsync(mapper.Map<Domain.Models.Role>(request), cancellationToken);

        var role = RoleAggregate.Create(request.Id, group.Id, request.Name, request.Description, request.IsActive);

        await repository.CreateAsync(role, cancellationToken);
    }
}