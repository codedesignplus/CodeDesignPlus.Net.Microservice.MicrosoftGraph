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
    /// Full name of the directory extension attribute that stores the user's document number,
    /// e.g. extension_{appIdWithoutHyphens}_DocumentNumber. Case-sensitive: it must match the
    /// property registered in the directory exactly.
    /// </summary>
    [Required]
    public string DocumentNumberClaim { get; set; } = null!;
    /// <summary>
    /// Full name of the directory extension attribute that stores the user's document type code,
    /// e.g. extension_{appIdWithoutHyphens}_DocumentType. Case-sensitive: it must match the
    /// property registered in the directory exactly.
    /// </summary>
    [Required]
    public string DocumentTypeClaim { get; set; } = null!;
    /// <summary>
    /// Full name of the directory extension attribute that stores the user's phone number,
    /// e.g. extension_{appIdWithoutHyphens}_Phone. Case-sensitive: it must match the
    /// property registered in the directory exactly.
    /// </summary>
    /// <remarks>
    /// The phone does not live in <c>mobilePhone</c>. The Entra External ID sign-up flow writes whatever the
    /// user typed into this extension attribute, so reading <c>mobilePhone</c> returns null for anyone who
    /// registered through the portal, and writing it leaves two phones that disagree: the one the user sees
    /// on the sign-up form and the one the platform shows.
    /// </remarks>
    [Required]
    public string PhoneClaim { get; set; } = null!;
}
