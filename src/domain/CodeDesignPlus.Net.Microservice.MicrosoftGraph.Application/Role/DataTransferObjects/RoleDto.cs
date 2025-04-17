namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.DataTransferObjects;

public class RoleDto : IDtoBase
{
    public required Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
}