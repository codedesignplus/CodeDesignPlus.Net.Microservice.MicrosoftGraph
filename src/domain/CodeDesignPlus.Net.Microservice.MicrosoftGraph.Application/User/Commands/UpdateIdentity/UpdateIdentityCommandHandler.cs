using System.Security.Principal;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;

public class UpdateIdentityCommandHandler(IUserRepository repository, IMapper mapper, IIdentityServer identityServer) : IRequestHandler<UpdateIdentityCommand>
{
    public async Task Handle(UpdateIdentityCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var user = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(user, Errors.UserNotFound);

        var userExist = await identityServer.GetUserByIdAsync(user.Id, cancellationToken);

        ApplicationGuard.IsNull(userExist, Errors.UserNotExistInIdentityServer);

        await identityServer.UpdateUserAsync(user.IdentityProviderId, mapper.Map<Domain.Models.User>(request), cancellationToken);
    }
}