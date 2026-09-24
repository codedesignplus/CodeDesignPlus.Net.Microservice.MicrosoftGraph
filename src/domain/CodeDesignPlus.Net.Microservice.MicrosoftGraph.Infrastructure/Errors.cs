using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("300", "UnknownError");
    public static readonly Error UserCreationFailed = new("301", "Cannot create user in Microsoft Graph");
    public static readonly Error EmailIsRequired = new("302", "Email is required for user creation.");
    public static readonly Error GivenNameIsRequired = new("303", "Given name is required for user creation.");
    public static readonly Error SurnameIsRequired = new("304", "Surname is required for user creation.");
    public static readonly Error PhoneIsRequired = new("305", "Phone number is required for user creation.");
    public static readonly Error InvalidSignUpInfo = new("306", "Invalid user sign-up information.");
    public static readonly Error UserNotFound = new("307", "User not found");
    public static readonly Error DocumentNumberIsRequired = new("308", "Document number is required for user creation.");
}
