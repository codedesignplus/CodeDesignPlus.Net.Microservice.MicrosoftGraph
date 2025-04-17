namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;

public class GraphOptions
{
    public const string Section = "Graph";
    /// <summary>
    /// The URL of the Graph API.
    /// </summary>
    public string Url { get; set; } = null!;
    /// <summary>
    /// The URL of the Graph API for the tenant.
    /// </summary>
    public string ClientId { get; set; } = null!;
    /// <summary>
    /// The URL of the Graph API for the tenant.
    /// </summary>
    public string ClientSecret { get; set; } = null!;
    /// <summary>
    /// The URL of the Graph API for the tenant.
    /// </summary>
    public string TenantId { get; set; } = null!;
}
