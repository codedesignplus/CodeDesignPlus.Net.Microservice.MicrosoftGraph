namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "100 : UnknownError";
    public const string NameRequired = "101 : The name is required";
    public const string DescriptionRequired = "102 : The description is required";
    public const string IdIsInvalid = "103 : The id is invalid";
    public const string IdIdentityServerIsInvalid = "104 : The id is invalid";
}
