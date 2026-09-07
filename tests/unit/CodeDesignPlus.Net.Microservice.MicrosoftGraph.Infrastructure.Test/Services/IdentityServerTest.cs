using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.IdentityServer;
using Microsoft.Graph.Models.ODataErrors;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Options;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Test.Services;

/// <summary>
/// Cubre el criterio que distingue "ya era miembro" de un fallo de verdad al anadir a alguien a un grupo.
/// </summary>
/// <remarks>
/// Graph responde 400 cuando el usuario ya pertenece al grupo, y eso viajaba como error de infraestructura:
/// cuatro reintentos y a la cola de descarte. Le ocurre a cualquier administrador que compre una segunda
/// licencia, porque el rol ya se lo dio la primera. Medido el 2026-08-19 al crear Malpelo XIX.
/// </remarks>
public class IdentityServerTest
{
    [Fact]
    public void ElDuplicadoNoEsUnFallo()
    {
        var error = Error(400, "One or more added object references already exist for the following modified properties: 'members'.");

        Assert.True(IdentityServer.YaEsMiembro(error));
    }

    [Fact]
    public void OtroErrorDe400SigueFallando()
    {
        // Es la mitad que importa del criterio: tragarse todos los 400 esconderia un grupo inexistente o una
        // peticion mal formada, y el usuario se quedaria sin su rol sin que nadie se entere.
        var error = Error(400, "Invalid object identifier 'no-es-un-guid'.");

        Assert.False(IdentityServer.YaEsMiembro(error));
    }

    [Fact]
    public void UnErrorDePermisosSigueFallando()
    {
        var error = Error(403, "Insufficient privileges to complete the operation.");

        Assert.False(IdentityServer.YaEsMiembro(error));
    }

    [Fact]
    public void UnErrorSinMensajeSigueFallando()
    {
        var error = new ODataError { ResponseStatusCode = 400 };

        Assert.False(IdentityServer.YaEsMiembro(error));
    }

    [Fact]
    public void ElCriterioNoDependeDeLasMayusculas()
    {
        // El texto lo escribe Microsoft y puede cambiarle la forma sin avisar.
        var error = Error(400, "One or more added object references ALREADY EXIST for 'members'.");

        Assert.True(IdentityServer.YaEsMiembro(error));
    }

    [Fact]
    public void ElDocumentoViajaConSuTipoEnCodigo()
    {
        var atributos = IdentityServer.AtributosDeDocumento(Opciones(), Usuario(new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC")));

        // El nombre de la clave sale de configuracion: cambia con el tenant y no se compone en el codigo.
        Assert.Equal("79800700", atributos["extension_appid_DocumentNumber"]);
        // Del tipo viaja el codigo, no el nombre ni el id: es la mitad estable del catalogo y la unica
        // que sirve fuera de esta base de datos.
        Assert.Equal("CC", atributos["extension_appid_DocumentType"]);
    }

    [Fact]
    public void SinTipoDeDocumentoLaClaveViajaEnNulo()
    {
        // El tipo es opcional. La clave se manda igual con null, que en Graph significa "borra el
        // atributo": omitirla dejaria en el directorio el tipo anterior de alguien que acaba de quitarlo.
        var atributos = IdentityServer.AtributosDeDocumento(Opciones(), Usuario(null));

        Assert.True(atributos.ContainsKey("extension_appid_DocumentType"));
        Assert.Null(atributos["extension_appid_DocumentType"]);
        Assert.Equal("79800700", atributos["extension_appid_DocumentNumber"]);
    }

    private static GraphOptions Opciones() => new()
    {
        DocumentNumberClaim = "extension_appid_DocumentNumber",
        DocumentTypeClaim = "extension_appid_DocumentType",
    };

    private static Domain.Models.User Usuario(DocumentType? tipo) => new()
    {
        DocumentNumber = "79800700",
        DocumentType = tipo,
    };

    private static ODataError Error(int codigo, string mensaje)
        => new() { ResponseStatusCode = codigo, Error = new MainError { Message = mensaje } };
}
