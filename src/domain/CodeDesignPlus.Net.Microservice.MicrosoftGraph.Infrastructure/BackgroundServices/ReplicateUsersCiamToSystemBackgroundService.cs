using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.ReplicateUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Commands.UpdateUserReplicate;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Queries.GetUsersPendingReplicate;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.BackgroundServices;

public class ReplicateUsersCiamToSystemBackgroundService(IMediator mediator, IIdentityServer identityServer, ILogger<ReplicateUsersCiamToSystemBackgroundService> logger) : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var users = await mediator.Send(new GetUsersPendingReplicateQuery(), stoppingToken);

            foreach (var user in users)
            {
                var userCiam = await identityServer.GetUserByEmailAsync(user.Email, stoppingToken);

                if (userCiam == null)
                    continue;

                if (string.IsNullOrEmpty(userCiam.Phone))
                {
                    await identityServer.UpdateUserPhoneAsync(userCiam.Id, user.Phone, stoppingToken);
                    userCiam.Phone = user.Phone;
                }

                logger.LogWarning("Replicating user with email {Email} from CIAM to system, {@User}", user.Email, user);

                var command = new ReplicateUserCommand(
                    user.Id,
                    userCiam.Id,
                    Domain.Enums.IdentityProvider.MicrosoftEntraExternalId,
                    userCiam.FirstName,
                    userCiam.LastName,
                    userCiam.Email,
                    userCiam.Phone,
                    userCiam.DisplayName,
                    user.IsActive
                );

                await mediator.Send(command, stoppingToken);

                var updateCommand = new UpdateUserReplicateCommand(user.Id);

                await mediator.Send(updateCommand, stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
