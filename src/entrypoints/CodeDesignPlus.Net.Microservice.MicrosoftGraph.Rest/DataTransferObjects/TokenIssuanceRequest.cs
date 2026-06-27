using System;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;


public class TokenIssuanceRequest
{
    public TokenIssuanceData Data { get; set; } = null!;
}

public class TokenIssuanceData
{
    [JsonProperty("authenticationContext")]
    public AuthenticationContext AuthenticationContext { get; set; } = null!;
}

public class AuthenticationContext
{
    [JsonProperty("correlationId")]
    public string CorrelationId { get; set; } = string.Empty;
    [JsonProperty("user")]
    public User User { get; set; } = null!;
}

public class User
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("mail")]
    public string? Mail { get; set; }
    [JsonProperty("userPrincipalName")]
    public string? UserPrincipalName { get; set; }
    [JsonProperty("displayName")]
    public string? DisplayName { get; set; }
    [JsonProperty("givenName")]
    public string? GivenName { get; set; }
    [JsonProperty("surname")]
    public string? Surname { get; set; }
}