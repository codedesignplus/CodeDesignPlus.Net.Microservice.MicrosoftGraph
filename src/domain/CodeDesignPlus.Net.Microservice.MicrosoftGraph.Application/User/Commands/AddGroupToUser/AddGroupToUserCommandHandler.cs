using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;

public class AddGroupToUserCommandHandler(IUserRepository repository, IRoleRepository roleRepository, IIdentityServer identityServer, ICacheManager cacheManager) : IRequestHandler<AddGroupToUserCommand>
{
    public async Task Handle(AddGroupToUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var user = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

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