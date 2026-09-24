using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("200");

    public static readonly Error InvalidRequest = new("201");
    public static readonly Error RoleAlreadyExists = new("202");
    public static readonly Error RoleNotFound = new("203");
    public static readonly Error GroupAlreadyExistsInIdentityServer = new("204");
    public static readonly Error GroupNotFoundInIdentityServer = new("205");
    public static readonly Error UserNotExistInIdentityServer = new("206");

    public static readonly Error UserNotFound = new("207");

    public static readonly Error UserAlreadyExists = new("208");

    public static readonly Error SecretContextNotFound = new("209");
}
