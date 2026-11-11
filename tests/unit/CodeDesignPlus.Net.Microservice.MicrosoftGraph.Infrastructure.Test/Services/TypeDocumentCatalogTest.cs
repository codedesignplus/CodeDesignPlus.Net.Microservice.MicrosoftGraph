using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.TypeDocumentCatalog;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Test.Services;

/// <summary>
/// Los tipos de documento del pais, los mismos que siembra ms-catalogs y con los mismos ids (pendings/663).
/// </summary>
public class TypeDocumentCatalogTest
{
    [Fact]
    public void Load_Colombia_HasTheDianCodes()
    {
        // Act
        var types = TypeDocumentCatalog.Load("co");

        // Assert
        Assert.Equal(["11", "12", "13", "21", "22", "31", "41", "42", "47", "48", "50", "91"], types.Select(t => t.Code));
    }

    [Fact]
    public void Load_Colombia_IdsAreTheFixedOnesOfMsCatalogs()
    {
        // Act
        var types = TypeDocumentCatalog.Load("co");

        // Assert
        Assert.All(types, t => Assert.Equal(Guid.Parse($"c0d0c000-0000-0000-0000-0000000000{t.Code}"), t.Id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("xx")]
    public void Load_CountryWithoutFile_FailsToStart(string? country)
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => TypeDocumentCatalog.Load(country));
    }

    [Theory]
    [InlineData("31")]
    [InlineData(" 31 ")]
    public void FindByCode_KnownCode_ReturnsTheTypeWithItsFixedId(string code)
    {
        // Arrange
        var catalog = new TypeDocumentCatalog(TypeDocumentCatalog.Load("co"));

        // Act
        var type = catalog.FindByCode(code);

        // Assert
        Assert.Equal(new Domain.ValueObjects.DocumentType(Guid.Parse("c0d0c000-0000-0000-0000-000000000031"), "NIT", "31"), type);
    }

    [Theory]
    [InlineData("CC")]
    [InlineData("99")]
    [InlineData("")]
    public void FindByCode_UnknownCode_ReturnsNull(string code)
    {
        // Arrange
        var catalog = new TypeDocumentCatalog(TypeDocumentCatalog.Load("co"));

        // Act
        var type = catalog.FindByCode(code);

        // Assert
        Assert.Null(type);
    }
}
