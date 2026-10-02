using System.Linq;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.CreateUser;

/// <summary>
/// La contraseña temporal cumple siempre la politica de complejidad del proveedor de identidad: si le falta una clase de
/// caracter, el alta falla y se retrasa hasta el siguiente reintento.
/// </summary>
public class TemporaryPasswordGeneratorTest
{
    private const int Samples = 10_000;

    [Fact]
    public void Generate_AlwaysHasEveryCharacterClassAndTheLength()
    {
        for (var i = 0; i < Samples; i++)
        {
            var password = TemporaryPasswordGenerator.Generate();

            Assert.Equal(TemporaryPasswordGenerator.Length, password.Length);
            Assert.Contains(password, TemporaryPasswordGenerator.Uppercase.Contains);
            Assert.Contains(password, TemporaryPasswordGenerator.Lowercase.Contains);
            Assert.Contains(password, TemporaryPasswordGenerator.Digits.Contains);
            Assert.Contains(password, TemporaryPasswordGenerator.Symbols.Contains);
        }
    }

    [Fact]
    public void Generate_OnlyUsesAllowedCharacters()
    {
        var allowed = TemporaryPasswordGenerator.Uppercase + TemporaryPasswordGenerator.Lowercase
            + TemporaryPasswordGenerator.Digits + TemporaryPasswordGenerator.Symbols;

        for (var i = 0; i < Samples; i++)
            Assert.All(TemporaryPasswordGenerator.Generate(), c => Assert.Contains(c, allowed));
    }

    /// <summary>La clase no se deduce de la posicion: el primer caracter no es siempre una mayuscula.</summary>
    [Fact]
    public void Generate_DoesNotFixTheClassByPosition()
    {
        var firstIsAlwaysUppercase = Enumerable.Range(0, 200)
            .All(_ => TemporaryPasswordGenerator.Uppercase.Contains(TemporaryPasswordGenerator.Generate()[0]));

        Assert.False(firstIsAlwaysUppercase);
    }
}
