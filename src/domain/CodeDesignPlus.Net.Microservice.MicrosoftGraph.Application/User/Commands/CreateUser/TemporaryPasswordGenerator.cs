using System.Security.Cryptography;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;

/// <summary>
/// Genera la contraseña temporal con la que nace una cuenta en el proveedor de identidad.
/// </summary>
/// <remarks>
/// Antes eran 16 caracteres sacados al azar de un solo conjunto con <see cref="Random"/>: a veces faltaba una clase de
/// caracter, el proveedor rechazaba la contraseña por complejidad y el alta se retrasaba hasta el siguiente reintento
///. Ahora lleva siempre al menos una mayuscula, una minuscula, un digito y un simbolo, y sale de un
/// generador criptografico, que es lo que corresponde a una credencial.
/// </remarks>
public static class TemporaryPasswordGenerator
{
    public const int Length = 16;

    public const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    public const string Digits = "0123456789";
    public const string Symbols = "@#!$%&*()+";

    private static readonly string[] Classes = [Uppercase, Lowercase, Digits, Symbols];
    private static readonly string All = string.Concat(Classes);

    public static string Generate()
    {
        var characters = new char[Length];

        // Uno de cada clase en las primeras posiciones; el resto de cualquiera, y al final se mezcla todo para que la
        // clase no se pueda deducir de la posicion.
        for (var i = 0; i < Classes.Length; i++)
            characters[i] = Pick(Classes[i]);

        for (var i = Classes.Length; i < Length; i++)
            characters[i] = Pick(All);

        RandomNumberGenerator.Shuffle(characters.AsSpan());

        return new string(characters);
    }

    private static char Pick(string source) => source[RandomNumberGenerator.GetInt32(source.Length)];
}
