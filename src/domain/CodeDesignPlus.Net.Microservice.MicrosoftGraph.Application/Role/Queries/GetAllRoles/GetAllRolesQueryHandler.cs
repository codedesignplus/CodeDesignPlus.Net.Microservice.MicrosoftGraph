using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Queries.GetAllRoles;

public class GetAllGraphQueryHandler(IRoleRepository repository, IMapper mapper, IUserContext user) : IRequestHandler<GetAllRolesQuery, Pagination<RoleDto>>
{
    public Task<Pagination<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<Pagination<RoleDto>>(default!);
    }
}
