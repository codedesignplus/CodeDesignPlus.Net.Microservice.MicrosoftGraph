using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Vault.Abstractions;
using CodeDesignPlus.Net.Vault.Abstractions.Options;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;

public class CreateUserCommandHandler(IUserRepository repository, IMapper mapper, IIdentityServer identityServer, IPubSub pubSub, IVaultTransit vaultTransit, IOptions<VaultOptions> options) : IRequestHandler<CreateUserCommand>
{
    private const string KEY_SECRET_CONTEXT = "vault_transit_password_temp";

    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync(request.Email, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserAlreadyExists);

        (string password, string key, string ciphertext) = await GeneratePasswordAsync();

        var user = mapper.Map<Domain.Models.User>(request);
        user.Password = password;

        var idUser = await identityServer.CreateUserAsync(user, cancellationToken);

        var userAggregate = UserAggregate.Create(idUser, request.FirstName, request.LastName, request.Email, request.Phone, request.DisplayName, key, ciphertext, false, request.IsActive);

        await repository.CreateAsync(userAggregate, cancellationToken);

        await pubSub.PublishAsync(userAggregate.GetAndClearEvents(), cancellationToken);
    }

    private async Task<(string password, string key, string ciphertext)> GeneratePasswordAsync()
    {
        var isValidContext = options.Value.Transit.SecretContexts.TryGetValue(KEY_SECRET_CONTEXT, out var secretContext);

        ApplicationGuard.IsFalse(isValidContext, Errors.SecretContextNotFound);

        var password = GenerateRandomPassword();

        var (key, ciphertext) = await vaultTransit.EncryptAsync(password, secretContext);
        return (password, key, ciphertext);
    }

    private static string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#!$%&*()+";
        var random = new Random();
        return new string([.. Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)])]);
    }
}