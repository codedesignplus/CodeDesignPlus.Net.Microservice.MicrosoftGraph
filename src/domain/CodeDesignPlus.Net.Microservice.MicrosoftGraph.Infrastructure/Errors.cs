using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("300");
    public static readonly Error UserCreationFailed = new("301");
    public static readonly Error EmailIsRequired = new("302");
    public static readonly Error GivenNameIsRequired = new("303");
    public static readonly Error SurnameIsRequired = new("304");
    public static readonly Error PhoneIsRequired = new("305");
    public static readonly Error InvalidSignUpInfo = new("306");
    public static readonly Error UserNotFound = new("307");
    public static readonly Error DocumentNumberIsRequired = new("308");
}
