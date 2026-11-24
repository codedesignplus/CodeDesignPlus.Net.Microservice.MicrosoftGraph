namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.SetAccountEnabled;

/// <summary>
/// Habilita o deshabilita la cuenta del usuario en el proveedor de identidad.
/// </summary>
/// <param name="Id">El usuario, con su id de ms-users.</param>
/// <param name="Enabled">Si puede iniciar sesión.</param>
public record SetAccountEnabledCommand(Guid Id, bool Enabled) : IRequest;

/// <summary>
/// Valida el comando.
/// </summary>
public class Validator : AbstractValidator<SetAccountEnabledCommand>
{
    /// <summary>
    /// Exige el usuario.
    /// </summary>
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
