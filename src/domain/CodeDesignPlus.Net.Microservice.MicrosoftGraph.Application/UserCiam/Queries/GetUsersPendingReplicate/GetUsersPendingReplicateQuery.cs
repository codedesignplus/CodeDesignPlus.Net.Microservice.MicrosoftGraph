using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Queries.GetUsersPendingReplicate;

public record GetUsersPendingReplicateQuery() : IRequest<List<UserCiamDto>>;

