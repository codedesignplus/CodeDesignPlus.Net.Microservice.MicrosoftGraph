using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;

public class UpdateProfileCommandHandler(IUserRepository repository, IMapper mapper, IIdentityServer identityServer, ICacheManager cacheManager) : IRequestHandler<UpdateProfileCommand>
{
    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var user = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(user, Errors.UserNotFound);

        var userExist = await identityServer.GetUserByIdAsync(user.Id, cancellationToken);

        ApplicationGuard.IsNull(userExist, Errors.UserNotExistInIdentityServer);

        var userModel = mapper.Map<Domain.Models.User>(request);

        await identityServer.UpdateUserAsync(user.IdentityProviderId, userModel, cancellationToken);
        await identityServer.UpdateContactInfoAsync(user.IdentityProviderId, userModel.Contact, cancellationToken);
        await identityServer.UpdateJobInfoAsync(user.IdentityProviderId, userModel.Job, cancellationToken);
        await cacheManager.RemoveAsync(user.IdentityProviderId.ToString());
    }
}