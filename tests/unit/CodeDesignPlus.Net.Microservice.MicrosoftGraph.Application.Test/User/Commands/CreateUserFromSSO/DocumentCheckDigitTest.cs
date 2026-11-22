using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Options;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUserFromSSO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.User.Commands.CreateUserFromSSO;

/// <summary>
/// El registro no crea un usuario con un documento de un tipo que lleva dígito de verificación si viene sin guion o con
/// otro dígito, y la configuración del despliegue es la que dice qué tipos y con qué pesos.
/// </summary>
public class DocumentCheckDigitTest
{
    private static readonly IOptions<UserDocumentOptions> Configured = Microsoft.Extensions.Options.Options.Create(new UserDocumentOptions
    {
        CheckDigitDocumentTypes = ["31"],
        CheckDigitWeights = [3, 7, 13, 17, 19, 23, 29, 37, 41, 43, 47, 53, 59, 67, 71]
    });

    private static readonly DocumentType TaxId = new(Guid.NewGuid(), "Tax identification number", "31");

    private readonly Mock<IUserRepository> repository = new();

    [Theory]
    [InlineData("901888777", "117")]
    [InlineData("901888777-5", "118")]
    [InlineData("901.888.777-3", "119")]
    public async Task Handle_TaxIdThatBreaksTheFormat_ThrowsAndDoesNotSave(string documentNumber, string expectedCode)
    {
        // Arrange
        var command = new CreateUserFromSSOCommand("Joe", "Doe", "joe@fake.com", "3107545252", "Joe Doe", documentNumber, TaxId, true);
        var handler = new CreateUserFromSSOCommandHandler(repository.Object, Configured);

        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal(expectedCode, exception.Code);
        Assert.Equal(Layer.Domain, exception.Layer);
        repository.Verify(x => x.CreateAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TaxIdWithItsCheckDigit_Saves()
    {
        // Arrange
        var command = new CreateUserFromSSOCommand("Joe", "Doe", "joe@fake.com", "3107545252", "Joe Doe", "901888777-3", TaxId, true);
        var handler = new CreateUserFromSSOCommandHandler(repository.Object, Configured);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repository.Verify(x => x.CreateAsync(It.Is<UserAggregate>(user => user.DocumentNumber == "901888777-3"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Validate_TypesWithoutWeights_Fails()
    {
        // Arrange
        var options = new UserDocumentOptions { CheckDigitDocumentTypes = ["31"] };

        // Act
        var result = new UserDocumentOptionsValidator().Validate(null, options);

        // Assert
        Assert.True(result.Failed);
    }

    [Fact]
    public void Validate_WeightThatIsNotPositive_Fails()
    {
        // Arrange
        var options = new UserDocumentOptions { CheckDigitDocumentTypes = ["31"], CheckDigitWeights = [3, 0, 13] };

        // Act
        var result = new UserDocumentOptionsValidator().Validate(null, options);

        // Assert
        Assert.True(result.Failed);
    }

    [Fact]
    public void Validate_EmptyDocumentTypeCode_Fails()
    {
        // Arrange
        var options = new UserDocumentOptions { CheckDigitDocumentTypes = [" "], CheckDigitWeights = [3, 7] };

        // Act
        var result = new UserDocumentOptionsValidator().Validate(null, options);

        // Assert
        Assert.True(result.Failed);
    }

    [Fact]
    public void Validate_NoSection_Succeeds()
    {
        // Act
        var result = new UserDocumentOptionsValidator().Validate(null, new UserDocumentOptions());

        // Assert
        Assert.True(result.Succeeded);
    }

    /// <summary>
    /// El valor desplegado. ms-users tiene esta misma prueba con el mismo valor esperado: si una de las dos
    /// configuraciones cambia sin la otra, su prueba falla. Lo que este registro acepta lo tiene que aceptar ms-users.
    /// </summary>
    [Fact]
    public void Appsettings_UserDocument_IsTheSharedDeployedValue()
    {
        // Arrange
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "CodeDesignPlus.Net.Microservice.MicrosoftGraph.sln")))
        {
            directory = directory.Parent;
        }

        var configuration = new ConfigurationBuilder().AddJsonFile(Path.Combine(directory!.FullName, "src", "entrypoints", "CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest", "appsettings.json")).Build();

        // Act
        var options = configuration.GetSection(UserDocumentOptions.Section).Get<UserDocumentOptions>();

        // Assert
        Assert.NotNull(options);
        Assert.True(new UserDocumentOptionsValidator().Validate(null, options).Succeeded);
        Assert.Equal(["31"], options.CheckDigitDocumentTypes);
        Assert.Equal([3, 7, 13, 17, 19, 23, 29, 37, 41, 43, 47, 53, 59, 67, 71], options.CheckDigitWeights);
    }
}
