using System;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public JobInfo Job { get; set; } = null!;
    public ContactInfo Contact { get; set; } = null!;
    public bool IsActive { get; set; }
}

public class ContactInfo
{
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Phone { get; private set; }
    public string[] Email { get; private set; } = [];
}
public class JobInfo
{
    public string? JobTitle { get; private set; }
    public string? CompanyName { get; private set; }
    public string? Department { get; private set; }
    public string? EmployeeId { get; private set; }
    public string? EmployeeType { get; private set; }
    public Instant? EmployHireDate { get; private set; }
    public string? OfficeLocation { get; private set; }
}