namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "200 : UnknownError";

    public const string InvalidRequest = "201 : The request is invalid";
    public const string RoleAlreadyExists = "202 : The role already exists";
    public const string RoleNotFound = "203 : The role was not found";
    public const string GroupAlreadyExistsInIdentityServer = "204 : The group already exists in IdentityServer";
    public const string GroupNotFoundInIdentityServer = "205 : The group was not found in IdentityServer";
    public const string UserNotExistInIdentityServer = "206 : The user does not exist in IdentityServer";

    public const string UserNotFound = "207 : The user was not found";

    public const string UserAlreadyExists = "208 : The user already exists";
}
