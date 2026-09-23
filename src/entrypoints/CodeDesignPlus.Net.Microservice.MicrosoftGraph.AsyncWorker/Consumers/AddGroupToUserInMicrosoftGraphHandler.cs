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

        if (!exists)
        {
            logger.LogInformation("User {Id} not found locally. Skipping Graph operation.", data.AggregateId);
            return;
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
