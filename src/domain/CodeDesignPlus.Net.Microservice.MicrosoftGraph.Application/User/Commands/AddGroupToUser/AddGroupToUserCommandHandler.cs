using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;

public class AddGroupToUserCommandHandler(IUserRepository repository, IRoleRepository roleRepository, IIdentityServer identityServer) : IRequestHandler<AddGroupToUserCommand>
{
    public async Task Handle(AddGroupToUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var user = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(user, Errors.UserNotFound);

        var userExist = await identityServer.GetUserByIdAsync(user.Id, cancellationToken);

        ApplicationGuard.IsNull(userExist, Errors.UserNotExistInIdentityServer);


        var role = await roleRepository.GetByNameAsync(request.Role, cancellationToken);

        Guid idGroupIdentityServer;

        if (role != null)
        {
            idGroupIdentityServer = role.IdIdentityServer;
        }
        else
        {
            var group = await identityServer.GetGroupByNameAsync(request.Role, cancellationToken);
            idGroupIdentityServer = group.Id;
        }

        await identityServer.AddUserToGroupAsync(user.Id, idGroupIdentityServer, cancellationToken);

        user.AddRole(idGroupIdentityServer);

        await repository.UpdateAsync(user, cancellationToken);
    }
}