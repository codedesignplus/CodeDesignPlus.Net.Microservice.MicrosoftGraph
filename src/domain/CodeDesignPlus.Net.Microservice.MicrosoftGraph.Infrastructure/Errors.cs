namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "300 : UnknownError";

    public const string UserCreationFailed = "301 : Cannot create user in Microsoft Graph"; 
}
