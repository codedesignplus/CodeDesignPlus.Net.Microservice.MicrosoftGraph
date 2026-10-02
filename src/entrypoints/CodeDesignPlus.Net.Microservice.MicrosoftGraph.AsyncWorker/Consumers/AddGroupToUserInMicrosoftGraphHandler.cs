using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "addgrouptouser")]
public class AddGroupToUserInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<AddGroupToUserInMicrosoftGraphHandler> logger) : IEventHandler<RoleAddedToUserDomainEvent>
{
    public async Task HandleAsync(RoleAddedToUserDomainEvent data, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<Domain.UserAggregate>(data.AggregateId, token);

        // Invitar a alguien y darle un rol pasa en el mismo gesto: ms-users publica el alta y, uno o dos segundos
        // despues, el rol. El alta tarda mas, porque crea la cuenta en el proveedor de identidad antes de guardar el
        // usuario aqui, asi que el rol suele llegar primero. Antes se descartaba con un log informativo y la persona
        // se quedaba sin grupo para siempre: 7 de las 11 cuentas invitadas en Copropietarios el 2026-10-02
        // (pendings/224). Ahora se lanza una excepcion de infraestructura, no de negocio, para que el bus lo
        // reintente con su espera creciente; si la cuenta nunca llega, el mensaje acaba en dead-letter, a la vista.
        if (!exists)
        {
            logger.LogWarning("User {Id} not found locally yet. The role {Role} will be retried.", data.AggregateId, data.Role);

            throw new UserNotProvisionedYetException(data.AggregateId);
        }

        // data.TenantId no se usa, y no es un olvido: los grupos del proveedor de identidad son globales
        // al directorio, no existe "Administrador de tal copropiedad". Lo que este micro mantiene es la
        // union de los roles del usuario en todas sus copropiedades, que es lo que el frontend necesita
        // para pintar antes de saber cual se esta mirando. Quien decide por copropiedad es el directorio
        // de roles del SDK.
        var command = new AddGroupToUserCommand(data.AggregateId, data.Role);

        await mediator.Send(command, token);
    }
}
