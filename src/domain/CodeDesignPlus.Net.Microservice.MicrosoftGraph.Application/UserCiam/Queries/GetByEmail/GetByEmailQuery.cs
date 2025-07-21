using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Queries.GetByEmail;

public record GetByEmailQuery(string Email) : IRequest<UserCiamDto>;

