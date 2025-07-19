namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.DataTransferObjects;

public class UserCiamDto : IDtoBase
{
    public required Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? DisplayName { get; set; } = null!;
    public bool WasCreatedFromSSO { get; set; }
    public bool UserReplicated { get; set; }
    public bool IsActive { get; set; }
}