using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Queries.GetByIdentityProviderId;

public class GetByIdentityProviderIdHandler(IUserRepository repository, IMapper mapper, ICacheManager cacheManager) : IRequestHandler<GetByIdentityProviderIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetByIdentityProviderIdQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            return await cacheManager.GetAsync<UserDto>(request.Id.ToString());

        var user = await repository.GetByIdentityProviderId(request.Id, cancellationToken);

        ApplicationGuard.IsNull(user, Errors.UserNotFound);

        var dto = mapper.Map<UserDto>(user);

        await cacheManager.SetAsync(request.Id.ToString(), dto, TimeSpan.FromMinutes(30));
        
        return dto;
    }
}
