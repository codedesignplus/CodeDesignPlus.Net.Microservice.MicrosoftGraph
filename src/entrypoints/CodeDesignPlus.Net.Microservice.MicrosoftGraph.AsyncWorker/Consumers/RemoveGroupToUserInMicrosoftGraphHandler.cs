using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

[QueueName("User", "removegrouptouser")]
public class RemoveGroupToUserInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<RemoveGroupToUserInMicrosoftGraphHandler> logger) : IEventHandler<RoleRemovedToUserDomainEvent>
{
    public async Task HandleAsync(RoleRemovedToUserDomainEvent data, CancellationToken token)
    {
        // El grupo del proveedor de identidad es global, asi que sacarlo de el por haber perdido el rol en
        // una sola copropiedad se lo quitaria en todas. Quien sabe si le queda en otra es ms-users, que
        // tiene el documento entero, y por eso lo dice en el evento.
        if (data.StillHasItElsewhere)
        {
            logger.LogInformation(
                "User {Id} keeps role {Role} in another tenant. Leaving the identity provider group untouched.",
                data.AggregateId, data.Role);

            return;
        }

        var exists = await userRepository.ExistsAsync<Domain.UserAggregate>(data.AggregateId, token);

        if (!exists)
        {
            logger.LogInformation("User {Id} not found locally. Skipping Graph operation.", data.AggregateId);
            return;
        }

        var command = new RemoveGroupToUserCommand(data.AggregateId, data.Role);

        await mediator.Send(command, token);
    }
}
