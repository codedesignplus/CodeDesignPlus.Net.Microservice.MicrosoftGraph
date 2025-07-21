using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Queries.GetByIdentityProviderId;

public class GetByIdentityProviderIdHandler(IUserRepository repository, IMapper mapper) : IRequestHandler<GetByIdentityProviderIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetByIdentityProviderIdQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var user = await repository.GetByIdentityProviderId(request.Id, cancellationToken);

        ApplicationGuard.IsNull(user, Errors.UserNotFound);

        return mapper.Map<UserDto>(user);
    }
}
