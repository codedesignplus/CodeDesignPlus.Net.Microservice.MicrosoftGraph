using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;

public class RemoveGroupToUserCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository, IIdentityServer identityServer, ICacheManager cacheManager) : IRequestHandler<RemoveGroupToUserCommand>
{
    public async Task Handle(RemoveGroupToUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var user = await userRepository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(user, Errors.UserNotFound);

        var userExist = await identityServer.GetUserByIdAsync(user.IdentityProviderId, cancellationToken);

        ApplicationGuard.IsNull(userExist, Errors.UserNotExistInIdentityServer);

        var role = await roleRepository.GetByNameAsync(request.Role, cancellationToken);

        Guid idGroupIdentityServer;

        if (role != null)
        {
            idGroupIdentityServer = role.IdIdentityServer;
        }
        else
        {
            var group = await identityServer.GetGroupByNameAsync(request.Role, cancellationToken);
            idGroupIdentityServer = group.Id;
        }

        await identityServer.RemoveUserFromGroupAsync(user.IdentityProviderId, idGroupIdentityServer, cancellationToken);

        // Se quita contra la base por la misma razon que se anade contra ella: reescribir el documento entero
        // devolveria a la vida los grupos que otra operacion acabara de anadir mientras esta leia.
        var quitado = await userRepository.RemoveRoleAsync(user.Id, idGroupIdentityServer, cancellationToken);

        // Si el grupo no estaba, el documento no cambio y la cache sigue coincidiendo con la base.
        if (!quitado)
            return;

        await cacheManager.RemoveAsync(user.IdentityProviderId.ToString());
    }
}