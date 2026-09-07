using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Setup;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.Setup;

public class MapsterConfigTest
{
    [Fact]
    public void Configure_ShouldMapProperties_Success()
    {
        // Arrange
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MapsterConfigGraph).Assembly);

        // Act
        var mapper = new Mapper(config);

        // Assert
        Assert.NotNull(mapper);
    }

    /// <summary>
    /// El documento sobrevive al mapeo de los tres comandos que acaban llamando a Graph.
    /// </summary>
    /// <remarks>
    /// El mapeo es por convencion de nombre —`NewConfig()` sin `.Map(...)`, y `UpdateProfileCommand`
    /// ni siquiera esta registrado—, asi que un cambio de nombre o de tipo en cualquiera de las dos
    /// puntas lo deja en null sin avisar y el atributo de extension se borra en el directorio.
    /// `Assert.NotNull(mapper)` no ve nada de eso.
    /// </remarks>
    [Theory]
    [MemberData(nameof(ComandosQueLleganAGraph))]
    public void ElDocumentoSobreviveAlMapeo(object comando)
    {
        // Se aplica la MISMA configuracion que el arranque (Startup.cs:12). `config.Scan` no vale:
        // busca implementaciones de IRegister y MapsterConfigGraph es una clase estatica, asi que
        // escaneando se prueba el mapeo por convencion y no el que corre en produccion.
        MapsterConfigGraph.Configure();
        var mapper = new Mapper(TypeAdapterConfig.GlobalSettings);

        var user = mapper.Map<Domain.Models.User>(comando);

        Assert.Equal("79800700", user.DocumentNumber);
        Assert.NotNull(user.DocumentType);
        Assert.Equal("CC", user.DocumentType.Code);
    }

    public static TheoryData<object> ComandosQueLleganAGraph()
    {
        var tipo = new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC");

        return new TheoryData<object>
        {
            new CreateUserCommand(Guid.NewGuid(), "Joe", "Doe", "joe.doe@fake.com", "3105631234", "Joe Doe", "79800700", tipo, true),
            new UpdateIdentityCommand(Guid.NewGuid(), "Joe", "Doe", "Joe Doe", "joe.doe@fake.com", "3105631234", "79800700", tipo, true),
            new UpdateProfileCommand(Guid.NewGuid(), "Joe", "Doe", "Joe Doe", "joe.doe@fake.com", "3105631234", "79800700", tipo, new ContactInfo(), new JobInfo(), true),
        };
    }
}
