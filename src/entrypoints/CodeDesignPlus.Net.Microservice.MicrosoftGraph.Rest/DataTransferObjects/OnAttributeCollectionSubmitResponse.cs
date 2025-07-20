using System;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;

public class OnAttributeCollectionSubmitResponse
{
    [JsonProperty("data")]
    public required Data Data { get; set; }

}

[JsonObject]
public class Data {
    [JsonProperty("@odata.type")]
    public required string Type { get; set; }
    [JsonProperty("actions")]
    public List<ModifiedAttributesAction> Actions { get; set; } = [];
}

[JsonObject]
public class ContinueWithDefaultBehavior {
    [JsonProperty("@odata.type")]
	public required string Type { get; set; }
}

[JsonObject]
public class ModifiedAttributesAction {
    [JsonProperty("@odata.type")]
    public required string Type { get; set; }
    [JsonProperty("attributes")]
    public Dictionary<string, object> Attributes { get; set; } = [];
}