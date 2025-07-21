using System;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;

public class TokenIssuanceResponse
{
    [JsonProperty("data")]
    public Data<ActionProviderClaim> Data { get; private set; } = new Data<ActionProviderClaim>
    {
        Type = "microsoft.graph.onTokenIssuanceStartResponseData",
        Actions = []
    };

    public static TokenIssuanceResponse Create() => new();
}

public class ActionProviderClaim(string correlationId, string userId)
{
    [JsonProperty("@odata.type")]
    public string Odatatype { get; set; } = "microsoft.graph.tokenIssuanceStart.provideClaimsForToken";
    [JsonProperty("claims")]
    public Claims Claims { get; set; } = new Claims(correlationId, userId);

    public static ActionProviderClaim Create(string correlationId, string userId)
    {
        return new ActionProviderClaim(correlationId, userId);
    }
}

public class Claims(string correlationId, string userId)
{
    [JsonProperty("correlationId")]
    public string CorrelationId { get; set; } = correlationId;
    [JsonProperty("userId")]
    public string UserId { get; set; } = userId;
}