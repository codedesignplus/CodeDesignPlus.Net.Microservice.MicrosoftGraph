using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;

public class AddGroupToUserCommandHandler(IUserRepository repository, IIdentityServer identityServer, ICacheManager cacheManager) : IRequestHandler<AddGroupToUserCommand>
{
    public async Task Handle(AddGroupToUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var user = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(user, Errors.UserNotFound);

        var userExist = await identityServer.GetUserByIdAsync(user.IdentityProviderId, cancellationToken);

        ApplicationGuard.IsNull(userExist, Errors.UserNotExistInIdentityServer);


        // El rol ya llega como identificador del grupo, asi que no hay nombre que traducir. Antes viajaba
        // el nombre y habia que buscarlo aqui o preguntarselo al proveedor de identidad.
        var idGroupIdentityServer = request.Role;

        await identityServer.AddUserToGroupAsync(user.IdentityProviderId, idGroupIdentityServer, cancellationToken);

        // Se anade contra la base, no contra el agregado que se leyo: dos asignaciones simultaneas sobre el
        // mismo usuario partian del mismo estado y la ultima en guardar borraba la otra.
        var anadido = await repository.AddRoleAsync(user.Id, idGroupIdentityServer, cancellationToken);

        // Si el grupo ya estaba, el documento no cambio y lo que hay en cache sigue siendo lo que hay en la
        // base: invalidarla solo obligaria a releer para obtener lo mismo. Se deja como esta.
        if (!anadido)
            return;

        await cacheManager.RemoveAsync(user.IdentityProviderId.ToString());
    }
}