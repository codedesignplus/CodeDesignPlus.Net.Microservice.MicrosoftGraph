using System.Reflection;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.TypeDocumentCatalog;

/// <summary>
/// Los tipos de documento del pais del despliegue, leidos del mismo fichero que siembra ms-catalogs (pendings/663).
/// </summary>
/// <remarks>
/// <para><b>Un despliegue, un pais.</b> El pais sale de <c>Seed:Country</c> y elige el fichero embebido
/// <c>seed-&lt;pais&gt;-type-documents.json</c>, igual que en ms-catalogs. Si no existe, el micro no arranca: resolver
/// contra los tipos de otro pais, o contra ninguno, rechazaria todos los registros sin decir por que.</para>
/// <para>El fichero es copia del de ms-catalogs, con los mismos ids y codigos: el tipo que se guarda en el usuario apunta
/// al registro sembrado alli. Cambiar uno sin el otro rompe esa referencia.</para>
/// </remarks>
public sealed class TypeDocumentCatalog : ITypeDocumentCatalog
{
    private readonly Dictionary<string, DocumentType> byCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeDocumentCatalog"/> class.
    /// </summary>
    /// <param name="types">The document types of the country.</param>
    public TypeDocumentCatalog(IEnumerable<TypeDocumentSeed> types)
    {
        ArgumentNullException.ThrowIfNull(types);

        byCode = types.ToDictionary(x => Normalize(x.Code), x => new DocumentType(x.Id, x.Name, x.Code));
    }

    /// <inheritdoc/>
    public DocumentType? FindByCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return byCode.GetValueOrDefault(Normalize(code));
    }

    /// <summary>
    /// Lee los tipos de documento del pais indicado. Falla si no hay pais o si no existe su fichero.
    /// </summary>
    /// <param name="country">The country of the deployment, e.g. "co".</param>
    /// <returns>The document types of the country.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the country is missing or has no file.</exception>
    public static List<TypeDocumentSeed> Load(string? country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new InvalidOperationException("Seed:Country no esta configurado: no se sabe contra que tipos de documento resolver el registro.");
        }

        var fileName = $"seed-{country.Trim().ToLowerInvariant()}-type-documents.json";
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(fileName, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"No existe {fileName}: el pais '{country}' no tiene tipos de documento.");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;

        return JsonSerializer.Deserialize<List<TypeDocumentSeed>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    private static string Normalize(string code) => code.Trim().ToUpperInvariant();
}

/// <summary>
/// Un tipo de documento del fichero sembrado del pais.
/// </summary>
/// <param name="Id">The fixed identifier, the same as in ms-catalogs.</param>
/// <param name="Name">The display name.</param>
/// <param name="Description">The description.</param>
/// <param name="Code">The country code; in Colombia, the DIAN code.</param>
public record TypeDocumentSeed(Guid Id, string Name, string? Description, string Code);
