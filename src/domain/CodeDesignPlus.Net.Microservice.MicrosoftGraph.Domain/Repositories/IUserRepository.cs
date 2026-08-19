namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;

public interface IUserRepository : IRepositoryBase
{
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken);
    Task<UserAggregate> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserAggregate> GetByIdentityProviderId(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Anade un grupo a la copia local del usuario sin reescribir el documento entero.
    /// </summary>
    /// <remarks>
    /// Los grupos los asignan procesos distintos que pueden coincidir en el tiempo: al registrar una
    /// titularidad y su residente llegan dos eventos y este microservicio los atiende a la vez. Leer el
    /// usuario, anadirle un grupo y guardarlo entero hace que el ultimo en escribir borre lo del otro
    /// — <b>y ocurrio</b>: el 2026-08-19 cuatro usuarios se quedaron con un solo grupo en la base local
    /// aunque en Entra tenian los dos, porque las dos asignaciones cayeron con milisegundos de diferencia.
    /// <para>
    /// Con <c>$addToSet</c> la base anade sobre el estado real, no sobre el que se leyo. Ademas no duplica,
    /// asi que reintentar la operacion es inofensivo.
    /// </para>
    /// </remarks>
    /// <returns><c>true</c> si el grupo no estaba y se anadio.</returns>
    Task<bool> AddRoleAsync(Guid id, Guid idRoleIdentityServer, CancellationToken cancellationToken);

    /// <summary>
    /// Quita un grupo de la copia local del usuario sin reescribir el documento entero.
    /// </summary>
    /// <remarks>
    /// Es el simetrico de <see cref="AddRoleAsync"/> y tenia la misma carrera al reves: quitar un grupo
    /// releyendo y reescribiendo el documento resucita los grupos que otra operacion acabara de anadir.
    /// <c>$pull</c> quita solo lo que se pide y deja el resto como este en la base.
    /// </remarks>
    /// <returns><c>true</c> si el grupo estaba y se quito.</returns>
    Task<bool> RemoveRoleAsync(Guid id, Guid idRoleIdentityServer, CancellationToken cancellationToken);
}
