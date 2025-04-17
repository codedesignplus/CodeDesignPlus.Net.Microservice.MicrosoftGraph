using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Models;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;

public class UserAggregate(Guid id) : AggregateRoot(id)
{    
    public Guid[] IdRoleAggregate { get; private set; } = [];

    public static UserAggregate Create(Guid id)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdIsInvalid);

        return new UserAggregate(id)
        {
        };
    }

    public void AddRole(Guid idRoleAggregate)
    {
        DomainGuard.GuidIsEmpty(idRoleAggregate, Errors.IdIsInvalid);

        DomainGuard.IsTrue(IdRoleAggregate.Contains(idRoleAggregate), Errors.RoleAlreadyAdded);

        IdRoleAggregate = [.. IdRoleAggregate, idRoleAggregate];
    }

    public void RemoveRole(Guid idRoleAggregate)
    {
        DomainGuard.GuidIsEmpty(idRoleAggregate, Errors.IdIsInvalid);

        DomainGuard.IsFalse(IdRoleAggregate.Contains(idRoleAggregate), Errors.RoleCannotBeRemoved);

        IdRoleAggregate = [.. IdRoleAggregate.Where(x => x != idRoleAggregate)];
    }
}
