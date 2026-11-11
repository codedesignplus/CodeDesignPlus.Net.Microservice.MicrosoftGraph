namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.TypeDocument.Queries.FindByCode;

/// <summary>
/// Resuelve el codigo de tipo de documento que llega del registro de Entra External ID contra los tipos sembrados del
/// pais (pendings/663).
/// </summary>
/// <param name="Code">The code chosen in the sign-up form; in Colombia, the DIAN code, e.g. "13".</param>
public record FindTypeDocumentByCodeQuery(string Code) : IRequest<DocumentType?>;
