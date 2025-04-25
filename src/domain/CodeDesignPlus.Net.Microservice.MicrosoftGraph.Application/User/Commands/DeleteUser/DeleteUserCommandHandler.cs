using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUserRepository repository, IIdentityServer identityServer) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        var userExist = await  identityServer.GetUserByIdAsync(aggregate.Id, cancellationToken);

        ApplicationGuard.IsNull(userExist, Errors.UserNotExistInIdentityServer);

        await identityServer.DeleteUserAsync(aggregate.Id, cancellationToken);

        await repository.DeleteAsync<UserAggregate>(aggregate.Id, cancellationToken);
    }
}