using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Vault.Abstractions;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;

public class CreateUserCommandHandler(IUserRepository repository, IMapper mapper, IIdentityServer identityServer, IPubSub pubSub, IVaultTransit vaultTransit, IOptions<GraphOptions> options) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserAlreadyExists);

        var password = GenerateRandomPassword();
        var (key, ciphertext)  = await vaultTransit.EncryptAsync(password, options.Value.SecretTransit);

        var userAggregate = UserAggregate.Create(request.Id, request.FirstName, request.LastName, request.Email, request.Phone, request.DisplayName, key, ciphertext, request.IsActive);

        await repository.CreateAsync(userAggregate, cancellationToken);

        var user = mapper.Map<Domain.Models.User>(request);
        user.Password = password;

        await identityServer.CreateUserAsync(user, cancellationToken);

        await pubSub.PublishAsync(userAggregate.GetAndClearEvents(), cancellationToken);
    }

    private static string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#!$%&*()+";
        var random = new Random();
        return new string([.. Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)])]);
    }
}