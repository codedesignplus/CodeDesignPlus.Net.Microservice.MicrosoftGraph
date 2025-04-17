using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateContactInfo;

public class UpdateContactInfoCommandHandler(IUserRepository repository, IMapper mapper, IIdentityServer identityServer) : IRequestHandler<UpdateContactInfoCommand>
{
    public async Task Handle(UpdateContactInfoCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var user = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(user, Errors.UserNotFound);

        var userExist = await identityServer.GetUserByIdAsync(user.Id, cancellationToken);

        ApplicationGuard.IsNull(userExist, Errors.UserNotExistInIdentityServer);

        await identityServer.UpdateContactInfoAsync(user.Id, mapper.Map<Domain.Models.ContactInfo>(request.Contact), cancellationToken);
    }
}