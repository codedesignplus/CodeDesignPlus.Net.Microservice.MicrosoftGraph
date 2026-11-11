using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Exceptions.Extensions;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.TypeDocument.Queries.FindActiveByCode;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUserFromSSO;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;
using Microsoft.Extensions.Logging.Abstractions;
using InfrastructureErrors = CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Errors;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.Test.Controllers;

/// <summary>
/// El registro de Entra External ID trae el codigo del tipo de documento y el micro lo exige y lo resuelve contra la copia
/// del catalogo (pendings/663).
/// </summary>
public class IdentityProviderControllerTest
{
    private const string DocumentTypeKey = "extension_0b1c2d3e4f5a6b7c8d9e0f1a2b3c4d5e_DocumentType";

    private readonly Mock<IMediator> mediator = new();
    private readonly IdentityProviderController controller;

    public IdentityProviderControllerTest()
    {
        controller = new IdentityProviderController(mediator.Object, NullLogger<IdentityProviderController>.Instance);
    }

    [Fact]
    public async Task CreateUser_WithKnownDocumentType_CreatesTheUserWithTheResolvedType()
    {
        // Arrange
        var cedula = new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC");
        mediator.Setup(x => x.Send(It.Is<FindActiveTypeDocumentByCodeQuery>(q => q.Code == "CC"), It.IsAny<CancellationToken>())).ReturnsAsync(cedula);

        // Act
        await controller.CreateUser(BuildRequest("CC"), CancellationToken.None);

        // Assert
        mediator.Verify(x => x.Send(It.Is<CreateUserFromSSOCommand>(c => c.DocumentType == cedula && c.DocumentNumber == "1023456781"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task CreateUser_WithoutDocumentType_ThrowsDocumentTypeIsRequired(string? code)
    {
        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => controller.CreateUser(BuildRequest(code), CancellationToken.None));

        // Assert
        Assert.Equal(InfrastructureErrors.DocumentTypeIsRequired.GetCode(), exception.Code);
    }

    [Fact]
    public async Task CreateUser_WithUnknownDocumentType_ThrowsDocumentTypeNotFound()
    {
        // Arrange
        mediator.Setup(x => x.Send(It.IsAny<FindActiveTypeDocumentByCodeQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((DocumentType?)null);

        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => controller.CreateUser(BuildRequest("XX"), CancellationToken.None));

        // Assert
        Assert.Equal(InfrastructureErrors.DocumentTypeNotFound.GetCode(), exception.Code);
        mediator.Verify(x => x.Send(It.IsAny<CreateUserFromSSOCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static OnAttributeCollectionSubmitRequest BuildRequest(string? documentTypeCode)
    {
        var attributes = new Dictionary<string, UserSignUpInfoAttribute>
        {
            ["givenName"] = new() { Value = "Jhon Fredy" },
            ["surname"] = new() { Value = "Castaño Ríos" },
            ["extension_0b1c2d3e4f5a6b7c8d9e0f1a2b3c4d5e_Phone"] = new() { Value = "+573105550131" },
            ["extension_0b1c2d3e4f5a6b7c8d9e0f1a2b3c4d5e_DocumentNumber"] = new() { Value = "1023456781" },
        };

        if (documentTypeCode is not null)
        {
            attributes[DocumentTypeKey] = new() { Value = documentTypeCode };
        }

        return new OnAttributeCollectionSubmitRequest
        {
            Data = new OnAttributeCollectionSubmitData
            {
                UserSignUpInfo = new UserSignUpInfo
                {
                    Attributes = attributes,
                    Identities = [new Identity { SignInType = "emailAddress", IssuerAssignedId = "porteria@fake.com" }],
                },
            },
        };
    }
}
