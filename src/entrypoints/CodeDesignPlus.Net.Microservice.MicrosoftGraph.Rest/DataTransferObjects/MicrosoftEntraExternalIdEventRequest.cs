using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;

public class MicrosoftEntraExternalIdEventRequest<TData> where TData : class
{
    public TData Data { get; set; } = null!;
}
