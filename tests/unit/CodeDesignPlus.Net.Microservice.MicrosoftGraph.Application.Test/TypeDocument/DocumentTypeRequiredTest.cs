using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.TypeDocument.Queries.FindByCode;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;
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
/// los tipos sembrados del pais (pendings/663).
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
    public async Task FindTypeDocumentByCode_KnownCode_ReturnsTheCountryType()
    {
        // Arrange
        var cedula = new DocumentType(Guid.Parse("c0d0c000-0000-0000-0000-000000000013"), "Cédula de Ciudadanía", "13");
        var catalog = new Mock<ITypeDocumentCatalog>();
        catalog.Setup(x => x.FindByCode("13")).Returns(cedula);
        var handler = new FindTypeDocumentByCodeQueryHandler(catalog.Object);

        // Act
        var result = await handler.Handle(new FindTypeDocumentByCodeQuery("13"), CancellationToken.None);

        // Assert
        Assert.Equal(cedula, result);
    }
}
