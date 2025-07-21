using System;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;


public class TokenIssuanceData
{

    [JsonProperty("authenticationContext")]
    public AuthenticationContext AuthenticationContext { get; set; } = null!;
}

public class AuthenticationContext
{
    [JsonProperty("correlationId")]
    public required string CorrelationId { get; set; }
}