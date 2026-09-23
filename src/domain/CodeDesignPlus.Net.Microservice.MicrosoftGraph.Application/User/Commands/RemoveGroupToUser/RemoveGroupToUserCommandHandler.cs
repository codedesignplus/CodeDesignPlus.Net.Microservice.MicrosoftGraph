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

        // El rol llega con el identificador del catalogo de ms-roles, que es el mismo en todos los
        // entornos. El del grupo en el proveedor de identidad cambia con cada directorio, y este micro es
        // el unico que necesita conocerlo: lo tiene aqui mismo, porque guarda el rol con la clave de
        // ms-roles y el identificador del grupo al lado.
        var role = await roleRepository.FindAsync<RoleAggregate>(request.Role, cancellationToken);

        ApplicationGuard.IsNull(role, Errors.RoleNotFound);

        var idGroupIdentityServer = role.IdIdentityServer;

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