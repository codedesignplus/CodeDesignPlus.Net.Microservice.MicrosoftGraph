using System;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;

public class OnAttributeCollectionSubmitRequest
{
    public OnAttributeCollectionSubmitData Data { get; set; } = null!;
}


public class OnAttributeCollectionSubmitData
{
    [JsonProperty("@odata.type")]
    public string Type { get; set; } = string.Empty;
    [JsonProperty("userSignUpInfo")]
    public UserSignUpInfo? UserSignUpInfo { get; set; }

    /// <summary>
    /// El contexto del registro. De aquí sale el idioma del formulario: Entra no manda <c>Accept-Language</c>.
    /// </summary>
    [JsonProperty("authenticationContext")]
    public SignUpAuthenticationContext? AuthenticationContext { get; set; }
}

/// <summary>
/// Lo que Entra cuenta de la sesión en la que alguien se está registrando.
/// </summary>
public class SignUpAuthenticationContext
{
    /// <summary>
    /// El navegador de quien se registra.
    /// </summary>
    [JsonProperty("client")]
    public SignUpClient? Client { get; set; }
}

/// <summary>
/// El navegador de quien se registra.
/// </summary>
public class SignUpClient
{
    /// <summary>
    /// El idioma en el que Entra le está mostrando el formulario, por ejemplo <c>es-co</c>.
    /// </summary>
    [JsonProperty("locale")]
    public string? Locale { get; set; }
}

public class UserSignUpInfo
{
    public Dictionary<string, UserSignUpInfoAttribute> Attributes { get; set; } = [];

    [JsonProperty("identities")]
    public List<Identity> Identities { get; set; } = [];
}

public class UserSignUpInfoAttribute
{
    [JsonProperty("value")]
    public string? Value { get; set; }

    [JsonProperty("@odata.type")]
    public string? Type { get; set; }

    [JsonProperty("attributeType")]
    public string? AttributeType { get; set; }
}

public class Identity
{
    [JsonProperty("signInType")]
    public string? SignInType { get; set; }

    [JsonProperty("issuer")]
    public string? Issuer { get; set; }

    [JsonProperty("issuerAssignedId")]
    public string? IssuerAssignedId { get; set; }
}
