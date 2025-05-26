using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.DomainEvents;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;

public class UserAggregate(Guid id) : AggregateRootBase(id)
{
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string? DisplayName { get; private set; } = null!;
    public Guid[] IdRoles { get; private set; } = [];

    public UserAggregate(Guid id, string firstName, string lastName, string email, string phone, string? displayName, string passwordKey, string passwordCipher, bool isActive) : this(id)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdIsInvalid);
        DomainGuard.IsNullOrEmpty(firstName, Errors.FirstNameIsRequired);
        DomainGuard.IsNullOrEmpty(lastName, Errors.LastNameIsRequired);
        DomainGuard.IsNullOrEmpty(phone, Errors.PhoneIsRequired);
        DomainGuard.IsNullOrEmpty(passwordKey, Errors.PasswordIsRequired);
        DomainGuard.IsNullOrEmpty(passwordCipher, Errors.PasswordIsRequired);
        DomainGuard.IsNullOrEmpty(email, Errors.EmailIsInvalid);

        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        DisplayName = displayName;
        IsActive = isActive;
        CreatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(UserCreatedDomainEvent.Create(id, firstName, lastName, email, phone, displayName, passwordKey, passwordCipher, isActive));
    }

    public static UserAggregate Create(Guid id, string firstName, string lastName, string email, string phone, string? displayName, string passwordKey, string passwordCipher, bool isActive)
    {
        return new UserAggregate(id, firstName, lastName, email, phone, displayName, passwordKey, passwordCipher, isActive);
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
