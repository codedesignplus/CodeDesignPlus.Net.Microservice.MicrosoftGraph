namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Test;

/// <summary>
/// Todo usuario nace con tipo y numero de documento (pendings/663), y la copia del catalogo resuelve el codigo como lo
/// guarda ms-catalogs.
/// </summary>
public class DocumentTypeRequiredTest
{
    private static readonly DocumentType Cedula = new(Guid.NewGuid(), "Cédula de Ciudadanía", "CC");

    public static TheoryData<DocumentType?> MissingTypes => new()
    {
        null,
        new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", ""),
        new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "  "),
    };

    [Theory]
    [MemberData(nameof(MissingTypes))]
    public void Create_WithoutDocumentType_ThrowsDocumentTypeIsRequired(DocumentType? documentType)
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => UserAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), Domain.Enums.IdentityProvider.MicrosoftEntraExternalId, "Joe", "Doe", "joe@fake.com", "3107545252", "Joe Doe", "1023456781", documentType!, "key", "cipher", false, true));

        // Assert
        Assert.Equal(Errors.DocumentTypeIsRequired.GetCode(), exception.Code);
    }

    [Theory]
    [MemberData(nameof(MissingTypes))]
    public void CreateFromSSO_WithoutDocumentType_ThrowsDocumentTypeIsRequired(DocumentType? documentType)
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => UserAggregate.CreateFromSSO(Guid.NewGuid(), "Joe", "Doe", "joe@fake.com", "3107545252", "Joe Doe", "1023456781", documentType!, true));

        // Assert
        Assert.Equal(Errors.DocumentTypeIsRequired.GetCode(), exception.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void CreateFromSSO_WithoutDocumentNumber_ThrowsDocumentNumberIsRequired(string? documentNumber)
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => UserAggregate.CreateFromSSO(Guid.NewGuid(), "Joe", "Doe", "joe@fake.com", "3107545252", "Joe Doe", documentNumber!, Cedula, true));

        // Assert
        Assert.Equal(Errors.DocumentNumberIsRequired.GetCode(), exception.Code);
    }

    [Fact]
    public void CompleteProviderRegistration_UserFromSSO_AnnouncesTheDocumentType()
    {
        // Arrange
        var user = UserAggregate.CreateFromSSO(Guid.NewGuid(), "Joe", "Doe", "joe@fake.com", "3107545252", "Joe Doe", "1023456781", Cedula, true);

        // Act
        user.CompleteProviderRegistration(Guid.NewGuid());

        // Assert
        var created = Assert.IsType<DomainEvents.UserCreatedDomainEvent>(Assert.Single(user.GetAndClearEvents()));
        Assert.Equal(Cedula, created.DocumentType);
    }

    [Fact]
    public void TypeDocumentCreate_LowercaseCode_StoresItNormalized()
    {
        // Act
        var typeDocument = TypeDocumentAggregate.Create(Guid.NewGuid(), "Cédula de Ciudadanía", " cc ", true, false, Instant.FromUtc(2026, 11, 11, 15, 0));

        // Assert
        Assert.Equal("CC", typeDocument.ToDocumentType().Code);
    }

    [Fact]
    public void TypeDocumentCreate_Deleted_IsNotActive()
    {
        // Act
        var typeDocument = TypeDocumentAggregate.Create(Guid.NewGuid(), "Registro Civil", "RC", true, true, Instant.FromUtc(2026, 11, 11, 15, 0));

        // Assert
        Assert.False(typeDocument.IsActive);
    }
}
