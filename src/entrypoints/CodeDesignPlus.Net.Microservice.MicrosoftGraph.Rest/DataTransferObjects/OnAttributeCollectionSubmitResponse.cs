using System;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;

public class OnAttributeCollectionSubmitResponse
{
    [JsonProperty("data")]
    public Data<AttributeCollectionSubmitAction> Data { get; private set; } = new Data<AttributeCollectionSubmitAction>
    {
        Type = "microsoft.graph.onAttributeCollectionSubmitResponseData",
        Actions = []
    };

    public static OnAttributeCollectionSubmitResponse Create() => new OnAttributeCollectionSubmitResponse();
}

/// <summary>
/// Una acción con la que se le responde a Entra External ID al enviar el formulario de registro.
/// </summary>
/// <remarks>
/// El contrato es el de Microsoft (evento <c>OnAttributeCollectionSubmit</c> de las extensiones de autenticación
/// personalizadas): cada acción se distingue por su <c>@odata.type</c>.
/// </remarks>
[JsonObject]
public abstract class AttributeCollectionSubmitAction
{
    /// <summary>
    /// El tipo de la acción, como lo nombra Microsoft Graph.
    /// </summary>
    [JsonProperty("@odata.type", Order = -2)]
    public string Type { get; set; } = string.Empty;
}

[JsonObject]
public class ContinueWithDefaultBehavior : AttributeCollectionSubmitAction
{
    public ContinueWithDefaultBehavior()
    {
        Type = "microsoft.graph.attributeCollectionSubmit.continueWithDefaultBehavior";
    }

    public static ContinueWithDefaultBehavior Create() => new();

    public static ContinueWithDefaultBehavior Create(string type) => new() { Type = type };
}

/// <summary>
/// La acción con la que el registro vuelve al formulario y pinta un motivo junto a cada dato rechazado.
/// </summary>
[JsonObject]
public class ShowValidationError : AttributeCollectionSubmitAction
{
    /// <summary>
    /// Crea la acción con su tipo de Microsoft Graph.
    /// </summary>
    public ShowValidationError()
    {
        Type = "microsoft.graph.attributeCollectionSubmit.showValidationError";
    }

    /// <summary>
    /// El aviso general del formulario.
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// El motivo de cada dato rechazado, por el nombre del atributo tal como lo envió Entra.
    /// </summary>
    [JsonProperty("attributeErrors")]
    public Dictionary<string, string> AttributeErrors { get; set; } = [];

    /// <summary>
    /// Crea la acción para un solo dato rechazado.
    /// </summary>
    /// <param name="message">El aviso general del formulario.</param>
    /// <param name="attribute">El nombre del atributo tal como lo envió Entra.</param>
    /// <param name="reason">El motivo del rechazo.</param>
    /// <returns>La acción.</returns>
    public static ShowValidationError Create(string message, string attribute, string reason)
    {
        return new ShowValidationError { Message = message, AttributeErrors = { [attribute] = reason } };
    }
}
