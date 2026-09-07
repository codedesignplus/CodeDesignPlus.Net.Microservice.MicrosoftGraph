namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

/// <summary>
/// Local mirror of the document type reference published by ms-users.
///
/// <see cref="Id"/> and <see cref="Value"/> identify the ms-catalogs entry and carry its display
/// snapshot; <see cref="Code"/> is the half that reaches the directory — it is written to the
/// document type extension attribute, so a token consumer can tell a national id from a tax id
/// without calling back into the platform.
/// </summary>
/// <param name="Id">Identifier of the catalog entry in ms-catalogs.</param>
/// <param name="Value">Display name at the time of selection, e.g. "Cédula de Ciudadanía".</param>
/// <param name="Code">Stable catalog code, e.g. "CC", "NIT", "PP".</param>
public record DocumentType(Guid Id, string Value, string Code);
