using System;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;

public class OnAttributeCollectionSubmitResponse
{
    [JsonProperty("data")]
    public Data<ContinueWithDefaultBehavior> Data { get; private set; } = new Data<ContinueWithDefaultBehavior>
    {
        Type = "microsoft.graph.onAttributeCollectionSubmitResponseData",
        Actions = []
    };

    public static OnAttributeCollectionSubmitResponse Create() => new OnAttributeCollectionSubmitResponse();
}

[JsonObject]
public class ContinueWithDefaultBehavior
{
    [JsonProperty("@odata.type")]
    public string Type { get; set; } = "microsoft.graph.attributeCollectionSubmit.continueWithDefaultBehavior";

    public static ContinueWithDefaultBehavior Create() => new();
    
    public static ContinueWithDefaultBehavior Create(string type) => new() { Type = type };
}