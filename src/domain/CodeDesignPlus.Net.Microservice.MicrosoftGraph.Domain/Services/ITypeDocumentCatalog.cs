using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

/// <summary>
/// Los tipos de documento del pais del despliegue, los mismos que siembra ms-catalogs y con los mismos ids (pendings/663).
/// </summary>
/// <remarks>
/// El registro de Entra External ID solo trae el codigo que eligio la persona (en Colombia, el de la DIAN: 13, 31...). Con
/// esta lista se arma el <see cref="DocumentType"/> completo sin llamar a ms-catalogs en medio del registro.
/// </remarks>
public interface ITypeDocumentCatalog
{
    /// <summary>
    /// Busca el tipo de documento con ese codigo.
    /// </summary>
    /// <param name="code">The code chosen in the sign-up form, with or without surrounding spaces.</param>
    /// <returns>The document type, or <c>null</c> if the country catalog has no type with that code.</returns>
    DocumentType? FindByCode(string code);
}
