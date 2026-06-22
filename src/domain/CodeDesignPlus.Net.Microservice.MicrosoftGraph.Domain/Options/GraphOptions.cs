using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;

public class GraphOptions
{
    public const string Section = "Graph";
    /// <summary>
    /// The URL of the Graph API for the tenant.
    /// </summary>
    [Required]
    public string ClientId { get; set; } = null!;
    /// <summary>
    /// The URL of the Graph API for the tenant.
    /// </summary>
    [Required]
    public string ClientSecret { get; set; } = null!;
    /// <summary>
    /// The URL of the Graph API for the tenant.
    /// </summary>
    [Required]
    public string TenantId { get; set; } = null!;
    /// <summary>
    /// /// The URL of the Graph API for the tenant.
    /// </summary>
    public string[] Scopes { get; set; } = ["https://graph.microsoft.com/.default"];
    /// <summary>
    /// Specifies the issuer of the identity
    /// </summary>
    [Required]
    public string IssuerIdentity { get; set; } = null!;
    /// <summary>
    /// The Application (client) ID of the app registration used for extension attributes (without hyphens).
    /// </summary>
    [Required]
    public string ExtensionAppId { get; set; } = null!;
}
