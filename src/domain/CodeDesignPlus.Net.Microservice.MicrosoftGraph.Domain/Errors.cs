using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("100");
    public static readonly Error NameRequired = new("101");
    public static readonly Error DescriptionRequired = new("102");
    public static readonly Error IdIsInvalid = new("103");
    public static readonly Error IdIdentityServerIsInvalid = new("104");
    public static readonly Error ObjectIdIsInvalid = new("105");

    public static readonly Error RoleAlreadyAdded = new("106"); 
    public static readonly Error RoleCannotBeRemoved = new("107");

    public static readonly Error EmailIsInvalid = new("108");

    public static readonly Error FirstNameIsRequired = new("109");
    public static readonly Error LastNameIsRequired = new("110");
    public static readonly Error PhoneIsRequired = new("111");
    public static readonly Error PasswordIsRequired = new("112");

    public static readonly Error IdIdentityProviderIsInvalid = new("113");
    public static readonly Error IdentityProviderIsInvalid = new("114");
}
