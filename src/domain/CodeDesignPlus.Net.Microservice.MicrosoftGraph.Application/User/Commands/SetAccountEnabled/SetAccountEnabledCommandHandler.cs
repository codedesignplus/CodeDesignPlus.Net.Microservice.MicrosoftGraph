using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.SetAccountEnabled;

/// <summary>
/// Habilita o deshabilita la cuenta del usuario en el proveedor de identidad.
/// </summary>
/// <remarks>
/// Lo piden los eventos de ms-users <c>UserDeactivatedDomainEvent</c> y <c>UserReactivatedDomainEvent</c>: una cuenta que
/// se quedó sin nada deja de poder iniciar sesión sin borrarse, y vuelve a poder cuando recibe un rol.
/// </remarks>
/// <param name="repository">El repositorio de usuarios.</param>
/// <param name="identityServer">El proveedor de identidad.</param>
/// <param name="cacheManager">La caché de usuarios.</param>
public class SetAccountEnabledCommandHandler(IUserRepository repository, IIdentityServer identityServer, ICacheManager cacheManager) : IRequestHandler<SetAccountEnabledCommand>
{
    /// <summary>
    /// Cambia el estado de la cuenta.
    /// </summary>
    /// <param name="request">El comando.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>La tarea.</returns>
    public async Task Handle(SetAccountEnabledCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var user = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(user, Errors.UserNotFound);

        await identityServer.SetAccountEnabledAsync(user.IdentityProviderId, request.Enabled, cancellationToken);

        await cacheManager.RemoveAsync(user.IdentityProviderId.ToString());
    }
}
