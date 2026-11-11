namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.TypeDocument.Queries.FindActiveByCode;

/// <summary>
/// Devuelve el tipo de documento activo con ese codigo, o <c>null</c> si el catalogo no lo tiene o esta inactivo.
/// </summary>
public class FindActiveTypeDocumentByCodeQueryHandler(ITypeDocumentRepository repository) : IRequestHandler<FindActiveTypeDocumentByCodeQuery, DocumentType?>
{
    /// <inheritdoc/>
    public async Task<DocumentType?> Handle(FindActiveTypeDocumentByCodeQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return null;
        }

        var typeDocument = await repository.FindActiveByCodeAsync(request.Code, cancellationToken);

        return typeDocument?.ToDocumentType();
    }
}
