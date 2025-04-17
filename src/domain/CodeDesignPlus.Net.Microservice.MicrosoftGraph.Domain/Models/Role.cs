using System;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
}
