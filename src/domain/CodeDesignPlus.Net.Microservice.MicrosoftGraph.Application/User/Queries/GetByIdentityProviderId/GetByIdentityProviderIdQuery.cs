using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Queries.GetByIdentityProviderId;

public record GetByIdentityProviderIdQuery(Guid Id) : IRequest<UserDto>;

