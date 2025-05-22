using System;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public JobInfo Job { get; set; } = null!;
    public ContactInfo Contact { get; set; } = null!;
    public bool IsActive { get; set; }
}

public class ContactInfo
{
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? Phone { get; set; }
    public string[] Email { get; set; } = [];
}
public class JobInfo
{
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
    public string? Department { get; set; }
    public string? EmployeeId { get; set; }
    public string? EmployeeType { get; set; }
    public Instant? EmployHireDate { get; set; }
    public string? OfficeLocation { get; set; }
}