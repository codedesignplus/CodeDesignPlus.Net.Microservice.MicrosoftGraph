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
    public static readonly Error DocumentNumberIsRequired = new("115");
    public static readonly Error DocumentTypeIsRequired = new("116");

    /// <summary>
    /// El tipo de documento lleva dígito de verificación y el número vino sin guion. Argumentos: el número y su dígito.
    /// </summary>
    public static readonly Error DocumentCheckDigitIsMissing = new("117");

    /// <summary>
    /// El dígito de verificación no es el del número. Argumentos: el número, el dígito que le corresponde y el escrito.
    /// </summary>
    public static readonly Error DocumentCheckDigitDoesNotMatch = new("118");

    /// <summary>
    /// El número de un documento con dígito de verificación no es «solo dígitos, guion y un dígito».
    /// </summary>
    public static readonly Error DocumentNumberFormatIsInvalid = new("119");

    /// <summary>
    /// El agregado recibió el documento sin las reglas del dígito de verificación: es un error de programación.
    /// </summary>
    public static readonly Error DocumentRulesAreRequired = new("120");

    /// <summary>
    /// El número vino sin guion y su último dígito es el de verificación de los anteriores: puede traerlo pegado o no.
    /// Argumentos: lo escrito, lo escrito sin el último dígito, ese último dígito y el dígito de todo lo escrito.
    /// </summary>
    public static readonly Error DocumentCheckDigitMayBeAttached = new("121");
}
