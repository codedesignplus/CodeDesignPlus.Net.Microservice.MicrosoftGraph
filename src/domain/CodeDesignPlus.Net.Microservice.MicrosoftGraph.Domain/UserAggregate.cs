namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;

public class UserAggregate(Guid id) : AggregateRootBase(id)
{    
    public string Email { get; private set; } = null!;
    public Guid[] IdRoles { get; private set; } = [];

    public static UserAggregate Create(Guid id, string email)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdIsInvalid);
        DomainGuard.IsNullOrEmpty(email, Errors.EmailIsInvalid);

        return new UserAggregate(id);
    }

    public void AddRole(Guid idRoleIdentityServer)
    {
        DomainGuard.GuidIsEmpty(idRoleIdentityServer, Errors.IdIsInvalid);

        DomainGuard.IsTrue(IdRoles.Contains(idRoleIdentityServer), Errors.RoleAlreadyAdded);

        IdRoles = [.. IdRoles, idRoleIdentityServer];
    }

    public void RemoveRole(Guid idRoleIdentityServer)
    {
        DomainGuard.GuidIsEmpty(idRoleIdentityServer, Errors.IdIsInvalid);

        DomainGuard.IsFalse(IdRoles.Contains(idRoleIdentityServer), Errors.RoleCannotBeRemoved);

        IdRoles = [.. IdRoles.Where(x => x != idRoleIdentityServer)];
    }
}
