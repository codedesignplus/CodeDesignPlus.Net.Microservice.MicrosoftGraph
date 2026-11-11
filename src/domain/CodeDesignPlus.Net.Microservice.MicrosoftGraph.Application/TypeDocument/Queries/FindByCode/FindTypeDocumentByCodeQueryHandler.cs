using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.TypeDocument.Queries.FindByCode;

/// <summary>
/// Devuelve el tipo de documento del pais con ese codigo, o <c>null</c> si no existe.
/// </summary>
public class FindTypeDocumentByCodeQueryHandler(ITypeDocumentCatalog catalog) : IRequestHandler<FindTypeDocumentByCodeQuery, DocumentType?>
{
    /// <inheritdoc/>
    public Task<DocumentType?> Handle(FindTypeDocumentByCodeQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        return Task.FromResult(catalog.FindByCode(request.Code));
    }
}
