using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.AddGroupToUser;

/// <summary>
/// Cubre dos operaciones de grupo que caen a la vez sobre el mismo usuario.
/// </summary>
/// <remarks>
/// Registrar una titularidad y su residente dispara dos eventos distintos que este microservicio atiende a la
/// vez. Antes cada uno leia el usuario, le anadia su grupo y guardaba el documento entero: los dos partian del
/// mismo estado y el ultimo en escribir borraba al otro. Ocurrio el 2026-08-19 con milisegundos de diferencia
/// y cuatro usuarios se quedaron con un solo grupo en la base local, aunque en Entra tenian los dos — la
/// llamada al proveedor si se hacia siempre, lo que se perdia era la copia local.
/// </remarks>
public class AsignacionSimultaneaDeGruposTest
{
    private static readonly Guid Usuario = Guid.Parse("6d2d4d1e-5a2f-4a3f-9f2c-3a1c9b7d4e51");
    private static readonly Guid IdentidadEnEntra = Guid.Parse("b1f0c5a4-2e3d-4c6b-8a97-1d0e2f3a4b5c");
    private static readonly Guid GrupoPropietario = Guid.Parse("7c9a1b2d-3e4f-4a5b-8c6d-9e0f1a2b3c4d");
    private static readonly Guid GrupoResidente = Guid.Parse("2f8e7d6c-5b4a-4938-8271-6a5b4c3d2e1f");

    /// <summary>Imita el documento del usuario: una lista que la base modifica sin releerla entera.</summary>
    private sealed class Documento
    {
        public List<Guid> IdRoles { get; } = [];
    }

    /// <summary>
    /// Fuerza el solapamiento en vez de esperar a que la maquina lo produzca sola.
    /// </summary>
    /// <remarks>
    /// Las dos operaciones se citan en la llamada al proveedor de identidad, que es donde se solapan de
    /// verdad: ninguna pasa hasta que las dos han leido el usuario, asi que las dos parten del mismo estado.
    /// Despues se sueltan en orden de llegada, porque quien guarda ultimo es quien puede pisar al otro y la
    /// prueba necesita decidir quien es. Sin esto el resultado depende del planificador y la prueba pasaria
    /// unas veces con el fallo dentro.
    /// </remarks>
    private sealed class Cita
    {
        private readonly TaskCompletionSource losDosLeyeron = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource elPrimeroGuardo = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int llegadas;

        public async Task EsperarAsync()
        {
            var soyElSegundo = Interlocked.Increment(ref llegadas) == 2;

            if (soyElSegundo)
                losDosLeyeron.TrySetResult();

            await losDosLeyeron.Task;

            if (soyElSegundo)
                await elPrimeroGuardo.Task;
        }

        public void Guarde() => elPrimeroGuardo.TrySetResult();
    }

    [Fact]
    public async Task LosDosGruposSobrevivenAunqueSeAsignenALaVez()
    {
        var documento = new Documento();
        var cita = new Cita();
        var handler = new AddGroupToUserCommandHandler(
            Repositorio(documento, cita).Object, RepositorioDeRoles().Object, ProveedorDeIdentidad(cita).Object, Mock.Of<ICacheManager>());

        await Task.WhenAll(
            handler.Handle(new AddGroupToUserCommand(Usuario, "Propietario"), CancellationToken.None),
            handler.Handle(new AddGroupToUserCommand(Usuario, "Residente"), CancellationToken.None));

        Assert.Equal(2, documento.IdRoles.Count);
        Assert.Contains(GrupoPropietario, documento.IdRoles);
        Assert.Contains(GrupoResidente, documento.IdRoles);
    }

    [Fact]
    public async Task QuitarUnGrupoNoResucitaElQueSeAnadeALaVez()
    {
        // El simetrico: si quitar releyera y reescribiera el documento entero, devolveria a la vida el grupo
        // que la otra operacion acaba de anadir. Quitar se lanza en segundo lugar porque es el que se vigila
        // y solo el que guarda ultimo puede pisar al otro.
        var documento = new Documento();
        documento.IdRoles.Add(GrupoPropietario);

        var cita = new Cita();
        var repositorio = Repositorio(documento, cita);
        var roles = RepositorioDeRoles();
        var identidad = ProveedorDeIdentidad(cita);

        var anadir = new AddGroupToUserCommandHandler(repositorio.Object, roles.Object, identidad.Object, Mock.Of<ICacheManager>());
        var quitar = new RemoveGroupToUserCommandHandler(repositorio.Object, roles.Object, identidad.Object, Mock.Of<ICacheManager>());

        await Task.WhenAll(
            anadir.Handle(new AddGroupToUserCommand(Usuario, "Residente"), CancellationToken.None),
            quitar.Handle(new RemoveGroupToUserCommand(Usuario, "Propietario"), CancellationToken.None));

        Assert.Single(documento.IdRoles);
        Assert.Contains(GrupoResidente, documento.IdRoles);
    }

    [Fact]
    public async Task AsignarDosVecesElMismoGrupoNoLoDuplicaNiInvalidaLaCacheDeMas()
    {
        // Los consumidores reintentan, asi que la misma asignacion puede llegar mas de una vez. La segunda no
        // cambia el documento, asi que lo que hay en cache sigue coincidiendo con la base y borrarla solo
        // obligaria a releer para obtener lo mismo.
        var documento = new Documento();
        var cache = new Mock<ICacheManager>();
        var handler = new AddGroupToUserCommandHandler(
            Repositorio(documento, null).Object, RepositorioDeRoles().Object, ProveedorDeIdentidad(null).Object, cache.Object);

        await handler.Handle(new AddGroupToUserCommand(Usuario, "Propietario"), CancellationToken.None);
        await handler.Handle(new AddGroupToUserCommand(Usuario, "Propietario"), CancellationToken.None);

        Assert.Single(documento.IdRoles);
        cache.Verify(x => x.RemoveAsync(IdentidadEnEntra.ToString()), Times.Once);
    }

    /// <summary>
    /// Reconstruye el agregado a partir del documento, como haria una lectura de la base.
    /// </summary>
    private static UserAggregate Leer(Documento documento)
    {
        var usuario = UserAggregate.Create(
            Usuario, IdentidadEnEntra, Domain.Enums.IdentityProvider.MicrosoftEntraExternalId,
            "Joe", "Doe", "joe.doe@fake.com", "3107545252", "Joe Doe", "1234567890", null, null, null, false, true);

        Guid[] instantanea;

        lock (documento)
            instantanea = [.. documento.IdRoles];

        foreach (var grupo in instantanea)
            usuario.AddRole(grupo);

        return usuario;
    }

    /// <summary>
    /// Los dobles imitan a <c>$addToSet</c> y <c>$pull</c>: tocan solo el grupo que se les pide y dejan el
    /// resto como este en la base.
    /// </summary>
    private static Mock<IUserRepository> Repositorio(Documento documento, Cita? cita)
    {
        var repositorio = new Mock<IUserRepository>();

        repositorio
            .Setup(x => x.FindAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Leer(documento));

        repositorio
            .Setup(x => x.AddRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns((Guid _, Guid grupo, CancellationToken __) =>
            {
                bool anadido;

                lock (documento)
                {
                    anadido = !documento.IdRoles.Contains(grupo);

                    if (anadido)
                        documento.IdRoles.Add(grupo);
                }

                cita?.Guarde();

                return Task.FromResult(anadido);
            });

        repositorio
            .Setup(x => x.RemoveRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns((Guid _, Guid grupo, CancellationToken __) =>
            {
                bool quitado;

                lock (documento)
                    quitado = documento.IdRoles.Remove(grupo);

                cita?.Guarde();

                return Task.FromResult(quitado);
            });

        // Imita el reemplazo del documento entero. El codigo actual no lo usa: esta aqui para que, si alguien
        // vuelve a UpdateAsync, la prueba reproduzca la perdida real en vez de fallar por un metodo sin llamar.
        repositorio
            .Setup(x => x.UpdateAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()))
            .Returns((UserAggregate agregado, CancellationToken _) =>
            {
                lock (documento)
                {
                    documento.IdRoles.Clear();
                    documento.IdRoles.AddRange(agregado.IdRoles);
                }

                cita?.Guarde();

                return Task.CompletedTask;
            });

        return repositorio;
    }

    private static Mock<IRoleRepository> RepositorioDeRoles()
    {
        var roles = new Mock<IRoleRepository>();

        roles
            .Setup(x => x.GetByNameAsync("Propietario", It.IsAny<CancellationToken>()))
            .ReturnsAsync(RoleAggregate.Create(Guid.NewGuid(), GrupoPropietario, "Propietario", "Propietario", true));

        roles
            .Setup(x => x.GetByNameAsync("Residente", It.IsAny<CancellationToken>()))
            .ReturnsAsync(RoleAggregate.Create(Guid.NewGuid(), GrupoResidente, "Residente", "Residente", true));

        return roles;
    }

    private static Mock<IIdentityServer> ProveedorDeIdentidad(Cita? cita)
    {
        var identidad = new Mock<IIdentityServer>();

        identidad
            .Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Models.User { Id = Usuario });

        identidad
            .Setup(x => x.AddUserToGroupAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns((Guid _, Guid __, CancellationToken ___) => cita?.EsperarAsync() ?? Task.CompletedTask);

        identidad
            .Setup(x => x.RemoveUserFromGroupAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns((Guid _, Guid __, CancellationToken ___) => cita?.EsperarAsync() ?? Task.CompletedTask);

        return identidad;
    }
}
