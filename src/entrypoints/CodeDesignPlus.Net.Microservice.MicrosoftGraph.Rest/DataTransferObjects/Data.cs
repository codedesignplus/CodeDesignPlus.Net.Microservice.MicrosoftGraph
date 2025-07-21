using System;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;


[JsonObject]
public class Data<TActions> where TActions : class
{
    [JsonProperty("@odata.type")]
    public required string Type { get; set; }
    [JsonProperty("actions")]
    public List<TActions> Actions { get; set; } = [];
}