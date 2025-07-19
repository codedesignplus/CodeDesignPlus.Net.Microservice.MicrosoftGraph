namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;

public class UserCiamAggregate(Guid id) : AggregateRootBase(id)
{
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string? DisplayName { get; private set; } = null!;
    public bool WasCreatedFromSSO { get; private set; }
    public bool UserReplicated { get; private set; }

    public UserCiamAggregate(Guid id, string firstName, string lastName, string email, string phone, string? displayName, bool wasCreatedFromSSO, bool isActive) : this(id)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdIsInvalid);
        DomainGuard.IsNullOrEmpty(firstName, Errors.FirstNameIsRequired);
        DomainGuard.IsNullOrEmpty(lastName, Errors.LastNameIsRequired);
        DomainGuard.IsNullOrEmpty(phone, Errors.PhoneIsRequired);
        DomainGuard.IsNullOrEmpty(email, Errors.EmailIsInvalid);

        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        DisplayName = displayName;
        IsActive = isActive;
        WasCreatedFromSSO = wasCreatedFromSSO;
        CreatedAt = SystemClock.Instance.GetCurrentInstant();
    }

    public static UserCiamAggregate Create(Guid id, string firstName, string lastName, string email, string phone, string? displayName, bool wasCreatedFromSSO, bool isActive)
    {
        return new UserCiamAggregate(id, firstName, lastName, email, phone, displayName, wasCreatedFromSSO, isActive);
    }

    public void MarkAsReplicated()
    {
        UserReplicated = true;
    }
}