namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;

public class RoleAggregate(Guid id) : AggregateRootBase(id)
{
    public Guid IdIdentityServer { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    
    public static RoleAggregate Create(Guid id, Guid idIdentityServer, string name, string description, bool isActive)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdIsInvalid);
        DomainGuard.GuidIsEmpty(idIdentityServer, Errors.IdIdentityServerIsInvalid);
        DomainGuard.IsNullOrEmpty(name, Errors.NameRequired);
        DomainGuard.IsNullOrEmpty(description, Errors.DescriptionRequired);

        var role = new RoleAggregate(id)
        {
            Name = name,
            IdIdentityServer = idIdentityServer,
            Description = description,
            IsActive = isActive,
            CreatedAt = SystemClock.Instance.GetCurrentInstant(),            
        };

        return role;
    }

    public void Update(string name, string description, bool isActive)
    {
        DomainGuard.IsNullOrEmpty(name, Errors.NameRequired);
        DomainGuard.IsNullOrEmpty(description, Errors.DescriptionRequired);

        Name = name;
        Description = description;
        IsActive = isActive;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
    }
}
