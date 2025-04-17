using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Queries.GetRoleById;

public record GetRoleByIdQuery(Guid Id) : IRequest<RoleDto>;

