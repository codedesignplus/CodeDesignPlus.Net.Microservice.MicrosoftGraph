using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("100", "UnknownError");
    public static readonly Error NameRequired = new("101", "The name is required");
    public static readonly Error DescriptionRequired = new("102", "The description is required");
    public static readonly Error IdIsInvalid = new("103", "The id is invalid");
    public static readonly Error IdIdentityServerIsInvalid = new("104", "The id is invalid");
    public static readonly Error ObjectIdIsInvalid = new("105", "The object id is invalid");

    public static readonly Error RoleAlreadyAdded = new("106", "The role is already added"); 
    public static readonly Error RoleCannotBeRemoved = new("107", "The role cannot be removed");

    public static readonly Error EmailIsInvalid = new("108", "The email is invalid");

    public static readonly Error FirstNameIsRequired = new("109", "The first name is required");
    public static readonly Error LastNameIsRequired = new("110", "The last name is required");
    public static readonly Error PhoneIsRequired = new("111", "The phone is required");
    public static readonly Error PasswordIsRequired = new("112", "The password is required");

    public static readonly Error IdIdentityProviderIsInvalid = new("113", "The id identity provider is invalid");
    public static readonly Error IdentityProviderIsInvalid = new("114", "The identity provider is invalid");
}
