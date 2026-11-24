using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.SetAccountEnabled;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

/// <summary>
/// Deshabilita en Entra la cuenta que ms-users desactivó porque al usuario no le quedó nada: deja de poder iniciar sesión,
/// pero no se borra.
/// </summary>
[QueueName("User", "disableaccount")]
public class DisableAccountInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<DisableAccountInMicrosoftGraphHandler> logger) : IEventHandler<UserDeactivatedDomainEvent>
{
    /// <summary>
    /// Deshabilita la cuenta.
    /// </summary>
    /// <param name="data">El evento de ms-users.</param>
    /// <param name="token">Token de cancelación.</param>
    /// <returns>La tarea.</returns>
    public Task HandleAsync(UserDeactivatedDomainEvent data, CancellationToken token) => AccountState.SetAsync(mediator, userRepository, logger, data.AggregateId, false, token);
}

/// <summary>
/// Vuelve a habilitar en Entra la cuenta que ms-users reactivó al darle un rol.
/// </summary>
[QueueName("User", "enableaccount")]
public class EnableAccountInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<EnableAccountInMicrosoftGraphHandler> logger) : IEventHandler<UserReactivatedDomainEvent>
{
    /// <summary>
    /// Habilita la cuenta.
    /// </summary>
    /// <param name="data">El evento de ms-users.</param>
    /// <param name="token">Token de cancelación.</param>
    /// <returns>La tarea.</returns>
    public Task HandleAsync(UserReactivatedDomainEvent data, CancellationToken token) => AccountState.SetAsync(mediator, userRepository, logger, data.AggregateId, true, token);
}

/// <summary>
/// El cambio de estado que comparten los dos consumidores.
/// </summary>
internal static class AccountState
{
    public static async Task SetAsync(IMediator mediator, IUserRepository userRepository, ILogger logger, Guid userId, bool enabled, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<Domain.UserAggregate>(userId, token);

        if (!exists)
        {
            logger.LogInformation("User {Id} not found locally. Skipping Graph operation.", userId);

            return;
        }

        await mediator.Send(new SetAccountEnabledCommand(userId, enabled), token);
    }
}
