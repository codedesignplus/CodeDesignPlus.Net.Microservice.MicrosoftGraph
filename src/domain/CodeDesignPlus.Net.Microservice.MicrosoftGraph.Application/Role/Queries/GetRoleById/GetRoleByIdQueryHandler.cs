using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Queries.GetRoleById;

public class GetRoleByIdQueryHandler(IRoleRepository repository, IMapper mapper, IUserContext user) : IRequestHandler<GetRoleByIdQuery, RoleDto>
{
    public Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<RoleDto>(default!);
    }
}
