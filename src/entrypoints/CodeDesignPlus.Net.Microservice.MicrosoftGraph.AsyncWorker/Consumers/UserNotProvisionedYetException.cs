namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;

/// <summary>
/// El usuario todavia no existe en este micro porque su alta sigue en curso.
/// </summary>
/// <remarks>
/// No hereda de <c>CodeDesignPlusException</c> a proposito: el bus manda a dead-letter de inmediato los errores de
/// negocio y reintenta los demas con espera creciente, que es lo que hace falta mientras termina el alta.
/// </remarks>
public class UserNotProvisionedYetException(Guid userId)
    : Exception($"User {userId} is not provisioned in Microsoft Graph yet.")
{
    public Guid UserId { get; } = userId;
}
