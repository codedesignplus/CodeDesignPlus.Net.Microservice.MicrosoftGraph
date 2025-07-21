using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Enums;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.DataTransferObjects;

public class UserDto
{
    public Guid Id { get; set; }
    public Guid IdIdentityProvider { get; set; }
    public IdentityProvider IdentityProvider { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? DisplayName { get; set; } = null!;
    public Guid[] IdRoles { get; set; } = [];
    public bool WasCreatedFromSSO { get; set; }
}
