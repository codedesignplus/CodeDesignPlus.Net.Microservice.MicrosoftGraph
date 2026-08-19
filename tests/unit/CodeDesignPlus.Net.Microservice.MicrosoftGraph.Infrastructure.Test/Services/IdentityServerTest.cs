using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.IdentityServer;
using Microsoft.Graph.Models.ODataErrors;

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

    private static ODataError Error(int codigo, string mensaje)
        => new() { ResponseStatusCode = codigo, Error = new MainError { Message = mensaje } };
}
