using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Users;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

/// <summary>
/// Consumes the user-registration event published by ms-users (mirrored locally as
/// <see cref="UserRegisteredDomainEvent"/>) and provisions the identity in
/// Azure AD B2C through the local CreateUserCommand. The downstream chain (password
/// generation, encryption with Vault, PasswordTemp email) is already wired inside the
/// command handler.
/// </summary>
[QueueName("User", "createuser")]
public class CreateUserInMicrosoftGraphHandler(IMediator mediator, IUserRepository userRepository, ILogger<CreateUserInMicrosoftGraphHandler> logger) : IEventHandler<UserRegisteredDomainEvent>
{
    public async Task HandleAsync(UserRegisteredDomainEvent data, CancellationToken token)
    {
        // Idempotencia: si el aggregate ya existe en ms-microsoftgraph (por reintento
        // de RabbitMQ o por una doble emisión) no volvemos a provisionar Azure AD.
        var exists = await userRepository.ExistsAsync<Domain.UserAggregate>(data.AggregateId, token);

        if (exists)
        {
            logger.LogInformation("Graph user {UserId} already exists. Skipping provisioning.", data.AggregateId);
            return;
        }

        var command = new CreateUserCommand(
            data.AggregateId,
            data.FirstName,
            data.LastName,
            data.Email,
            data.Phone,
            data.DisplayName,
            data.DocumentNumber,
            data.IsActive
        );

        await mediator.Send(command, token);
    }
}
