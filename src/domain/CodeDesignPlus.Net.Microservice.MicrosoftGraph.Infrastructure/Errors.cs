namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "300 : UnknownError";
    public const string UserCreationFailed = "301 : Cannot create user in Microsoft Graph";
    public const string EmailIsRequired = "302 : Email is required for user creation.";
    public const string GivenNameIsRequired = "303 : Given name is required for user creation.";
    public const string SurnameIsRequired = "304 : Surname is required for user creation.";
    public const string PhoneIsRequired = "305 : Phone number is required for user creation.";
    public const string InvalidSignUpInfo = "306 : Invalid user sign-up information.";
}
