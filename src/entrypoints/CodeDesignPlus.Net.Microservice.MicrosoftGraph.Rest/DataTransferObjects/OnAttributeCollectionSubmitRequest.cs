using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;

public class OnAttributeCollectionSubmitRequest
{
    public OnAttributeCollectionSubmitData Data { get; set; } = null!;
}

public class OnAttributeCollectionSubmitData
{

    [JsonProperty("@odata.type")]
    public required string Type { get; set; }
    [JsonProperty("userSignUpInfo")]
    public UserSignUpInfo? UserSignUpInfo { get; set; } = null!;
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
    public required string Value { get; set; }

    [JsonProperty("@odata.type")]
    public required string Type { get; set; }

    [JsonProperty("attributeType")]
    public required string AttributeType { get; set; }
}

public class Identity
{
    [JsonProperty("signInType")]
    public required string SignInType { get; set; }

    [JsonProperty("issuer")]
    public required string Issuer { get; set; }

    [JsonProperty("issuerAssignedId")]
    public required string IssuerAssignedId { get; set; }
}