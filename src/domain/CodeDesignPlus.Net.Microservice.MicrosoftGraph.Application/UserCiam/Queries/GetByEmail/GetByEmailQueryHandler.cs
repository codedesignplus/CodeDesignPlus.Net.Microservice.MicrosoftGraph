using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Queries.GetByEmail;

public class GetByEmailQueryHandler(IUserCiamRepository repository, IMapper mapper) : IRequestHandler<GetByEmailQuery, UserCiamDto>
{
    public async Task<UserCiamDto> Handle(GetByEmailQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var user = await repository.GetByEmailAsync(request.Email, cancellationToken);

        ApplicationGuard.IsNotNull(user, Errors.UserNotFound);

        return mapper.Map<UserCiamDto>(user);
    }
}
