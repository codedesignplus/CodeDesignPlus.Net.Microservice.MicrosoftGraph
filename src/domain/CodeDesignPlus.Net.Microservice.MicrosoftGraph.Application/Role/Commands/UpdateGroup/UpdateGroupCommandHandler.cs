using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;

public class UpdateGroupCommandHandler(IRoleRepository repository, IMapper mapper, IIdentityServer identityServer) : IRequestHandler<UpdateGroupCommand>
{
    public  async Task Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var role = await repository.FindAsync<RoleAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(role, Errors.RoleNotFound);

        var groupExist = await identityServer.GetGroupByIdAsync(role.IdIdentityServer, cancellationToken);

        ApplicationGuard.IsNull(groupExist, Errors.GroupNotFoundInIdentityServer);

        await identityServer.UpdateRoleAsync(role.IdIdentityServer, mapper.Map<Domain.Models.Role>(request), cancellationToken);

        role.Update(request.Name, request.Description, request.IsActive);

        await repository.UpdateAsync(role, cancellationToken);
    }
}