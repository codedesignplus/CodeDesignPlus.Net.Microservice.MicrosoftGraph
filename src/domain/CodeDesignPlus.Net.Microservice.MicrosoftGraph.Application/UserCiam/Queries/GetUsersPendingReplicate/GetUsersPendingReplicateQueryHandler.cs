using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Queries.GetUsersPendingReplicate;

public class GetUsersPendingReplicateQueryHandler(IUserCiamRepository repository, IMapper mapper) : IRequestHandler<GetUsersPendingReplicateQuery, List<UserCiamDto>>
{
    public async Task<List<UserCiamDto>> Handle(GetUsersPendingReplicateQuery request, CancellationToken cancellationToken)
    {
        var users = await repository.GetUsersPendingReplicateAsync(cancellationToken);

        return mapper.Map<List<UserCiamDto>>(users);
    }
}
