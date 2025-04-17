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
}
