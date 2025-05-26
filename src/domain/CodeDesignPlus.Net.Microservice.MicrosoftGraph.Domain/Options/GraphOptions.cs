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
    /// Specifies the secret used for encrypting and decrypting values in the transit secret engine.
    /// </summary>
    [Required]
    public string SecretTransit { get; set; } = null!;
}
