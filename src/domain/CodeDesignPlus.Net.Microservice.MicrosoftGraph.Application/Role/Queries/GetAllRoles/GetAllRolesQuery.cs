using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Queries.GetAllRoles;

public record GetAllRolesQuery(Guid Id) : IRequest<Pagination<RoleDto>>;

