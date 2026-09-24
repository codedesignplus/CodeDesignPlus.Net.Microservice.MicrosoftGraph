using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("200", "UnknownError");

    public static readonly Error InvalidRequest = new("201", "The request is invalid");
    public static readonly Error RoleAlreadyExists = new("202", "The role already exists");
    public static readonly Error RoleNotFound = new("203", "The role was not found");
    public static readonly Error GroupAlreadyExistsInIdentityServer = new("204", "The group already exists in IdentityServer");
    public static readonly Error GroupNotFoundInIdentityServer = new("205", "The group was not found in IdentityServer");
    public static readonly Error UserNotExistInIdentityServer = new("206", "The user does not exist in IdentityServer");

    public static readonly Error UserNotFound = new("207", "The user was not found");

    public static readonly Error UserAlreadyExists = new("208", "The user already exists");

    public static readonly Error SecretContextNotFound = new("209", "The secret context was not found");
}
