using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.DeleteGroup;

public class DeleteGroupCommandHandler(IRoleRepository repository, IIdentityServer identityServer) : IRequestHandler<DeleteGroupCommand>
{
    public async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<RoleAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.RoleNotFound);

        var groupExist = await  identityServer.GetGroupByIdAsync(aggregate.IdIdentityServer, cancellationToken);

        ApplicationGuard.IsNull(groupExist, Errors.GroupNotFoundInIdentityServer);

        await identityServer.DeleteGroupAsync(aggregate.IdIdentityServer, cancellationToken);

        await repository.DeleteAsync<RoleAggregate>(aggregate.Id, cancellationToken);
    }
}