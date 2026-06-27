using System;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;

public class OnAttributeCollectionSubmitRequest
{
    public OnAttributeCollectionSubmitData Data { get; set; } = null!;
}


public class OnAttributeCollectionSubmitData
{
    [JsonProperty("@odata.type")]
    public string Type { get; set; } = string.Empty;
    [JsonProperty("userSignUpInfo")]
    public UserSignUpInfo? UserSignUpInfo { get; set; }
}

public class UserSignUpInfo
{
    public Dictionary<string, UserSignUpInfoAttribute> Attributes { get; set; } = [];

    [JsonProperty("identities")]
    public List<Identity> Identities { get; set; } = [];
}

public class UserSignUpInfoAttribute
{
    [JsonProperty("value")]
    public string? Value { get; set; }

    [JsonProperty("@odata.type")]
    public string? Type { get; set; }

    [JsonProperty("attributeType")]
    public string? AttributeType { get; set; }
}

public class Identity
{
    [JsonProperty("signInType")]
    public string? SignInType { get; set; }

    [JsonProperty("issuer")]
    public string? Issuer { get; set; }

    [JsonProperty("issuerAssignedId")]
    public string? IssuerAssignedId { get; set; }
}
