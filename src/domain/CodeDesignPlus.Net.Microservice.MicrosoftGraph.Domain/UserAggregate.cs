using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.DomainEvents;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Enums;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;

public class UserAggregate(Guid id) : AggregateRootBase(id)
{
    public Guid IdentityProviderId { get; private set; }
    public IdentityProvider IdentityProvider { get; private set; }
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string? DisplayName { get; private set; } = null!;
    public Guid[] IdRoles { get; private set; } = [];
    public bool WasCreatedFromSSO { get; private set; }

    public static UserAggregate Create(Guid id, Guid idIdentityProvider, IdentityProvider identityProvider, string firstName, string lastName, string email, string phone, string? displayName, string? passwordKey, string? passwordCipher, bool wasCreatedFromSSO, bool isActive)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdIsInvalid);
        DomainGuard.GuidIsEmpty(idIdentityProvider, Errors.IdIdentityProviderIsInvalid);
        DomainGuard.IsTrue(identityProvider == IdentityProvider.None, Errors.IdentityProviderIsInvalid);
        DomainGuard.IsNullOrEmpty(firstName, Errors.FirstNameIsRequired);
        DomainGuard.IsNullOrEmpty(lastName, Errors.LastNameIsRequired);
        DomainGuard.IsNullOrEmpty(phone, Errors.PhoneIsRequired);
        DomainGuard.IsNullOrEmpty(email, Errors.EmailIsInvalid);

        var aggregate = new UserAggregate(id)
        {
            IdentityProviderId = idIdentityProvider,
            IdentityProvider = identityProvider,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            DisplayName = displayName,
            WasCreatedFromSSO = wasCreatedFromSSO
        };

        aggregate.AddEvent(UserCreatedDomainEvent.Create(id, firstName, lastName, email, phone, displayName, passwordKey, passwordCipher, wasCreatedFromSSO, isActive));
        
        return aggregate;
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
