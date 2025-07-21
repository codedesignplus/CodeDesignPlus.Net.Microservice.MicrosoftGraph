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
    [JsonProperty("user")]
    public User User { get; set; } = null!;
}

public class AuthenticationContext
{
    [JsonProperty("correlationId")]
    public required string CorrelationId { get; set; }
}

public class User
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("mail")]
    public string Mail { get; set; } = null!;
    [JsonProperty("userPrincipalName")]
    public string UserPrincipalName { get; set; } = null!;
    [JsonProperty("displayName")]
    public string DisplayName { get; set; } = null!;
    [JsonProperty("givenName")]
    public string GivenName { get; set; } = null!;
    [JsonProperty("surname")]
    public string Surname { get; set; } = null!;
}