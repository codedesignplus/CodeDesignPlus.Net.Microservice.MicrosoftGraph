namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.TypeDocument.Queries.FindActiveByCode;

/// <summary>
/// Resuelve el codigo de tipo de documento que llega del registro de Entra External ID contra la copia local del
/// catalogo de ms-catalogs (pendings/663).
/// </summary>
/// <param name="Code">The catalog code chosen in the sign-up form, e.g. "CC".</param>
public record FindActiveTypeDocumentByCodeQuery(string Code) : IRequest<DocumentType?>;
