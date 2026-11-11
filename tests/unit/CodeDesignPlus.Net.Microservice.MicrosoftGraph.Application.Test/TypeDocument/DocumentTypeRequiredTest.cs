using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.TypeDocument.Queries.FindActiveByCode;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUserFromSSO;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.ReplicateUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Test.TypeDocument;

/// <summary>
/// Los comandos que crean o actualizan un usuario piden el tipo de documento, y el codigo del registro se resuelve contra
/// la copia del catalogo (pendings/663).
/// </summary>
public class DocumentTypeRequiredTest
{
    [Fact]
    public void CreateUserValidator_WithoutDocumentType_HasError()
    {
        // Arrange
        var command = new CreateUserCommand(Guid.NewGuid(), "Joe", "Doe", "joe@fake.com", "3107545252", "Joe Doe", "1023456781", null!, true);

        // Act
        var result = new Application.User.Commands.CreateUser.Validator().TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }

    [Fact]
    public void CreateUserFromSSOValidator_WithoutDocumentType_HasError()
    {
        // Arrange
        var command = new CreateUserFromSSOCommand("Joe", "Doe", "joe@fake.com", "3107545252", "Joe Doe", "1023456781", null!, true);

        // Act
        var result = new Application.User.Commands.CreateUserFromSSO.Validator().TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }

    [Fact]
    public void ReplicateUserValidator_WithoutDocumentType_HasError()
    {
        // Arrange
        var command = new ReplicateUserCommand(Guid.NewGuid(), Guid.NewGuid(), Domain.Enums.IdentityProvider.MicrosoftEntraExternalId, "Joe", "Doe", "joe@fake.com", "3107545252", "Joe Doe", "1023456781", null!, true);

        // Act
        var result = new Application.User.Commands.ReplicateUser.Validator().TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }

    [Fact]
    public void UpdateIdentityValidator_WithoutDocumentType_HasError()
    {
        // Arrange
        var command = new UpdateIdentityCommand(Guid.NewGuid(), "Joe", "Doe", "Joe Doe", "joe@fake.com", "3107545252", "1023456781", null!, true);

        // Act
        var result = new Application.User.Commands.UpdateIdentity.Validator().TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }

    [Fact]
    public void UpdateProfileValidator_WithoutDocumentType_HasError()
    {
        // Arrange
        var command = new UpdateProfileCommand(Guid.NewGuid(), "Joe", "Doe", "Joe Doe", "joe@fake.com", "3107545252", "1023456781", null!, new ContactInfo(), new JobInfo(), true);

        // Act
        var result = new Application.User.Commands.UpdateProfile.Validator().TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }

    [Fact]
    public async Task FindActiveTypeDocumentByCode_KnownCode_ReturnsTheCatalogEntry()
    {
        // Arrange
        var entry = TypeDocumentAggregate.Create(Guid.NewGuid(), "Número de Identificación Tributaria", "NIT", true, false, SystemClock.Instance.GetCurrentInstant());
        var repository = new Mock<ITypeDocumentRepository>();
        repository.Setup(x => x.FindActiveByCodeAsync("nit", It.IsAny<CancellationToken>())).ReturnsAsync(entry);
        var handler = new FindActiveTypeDocumentByCodeQueryHandler(repository.Object);

        // Act
        var result = await handler.Handle(new FindActiveTypeDocumentByCodeQuery("nit"), CancellationToken.None);

        // Assert
        Assert.Equal(new DocumentType(entry.Id, "Número de Identificación Tributaria", "NIT"), result);
    }

    [Fact]
    public async Task FindActiveTypeDocumentByCode_UnknownCode_ReturnsNull()
    {
        // Arrange
        var repository = new Mock<ITypeDocumentRepository>();
        var handler = new FindActiveTypeDocumentByCodeQueryHandler(repository.Object);

        // Act
        var result = await handler.Handle(new FindActiveTypeDocumentByCodeQuery("XX"), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
