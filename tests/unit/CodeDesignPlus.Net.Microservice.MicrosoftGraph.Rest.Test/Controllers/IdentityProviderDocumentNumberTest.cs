using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.TypeDocument.Queries.FindByCode;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUserFromSSO;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DomainErrors = CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Errors;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.Test.Controllers;

/// <summary>
/// Cuando el número de documento no cumple el formato de su tipo, el registro de Entra External ID no se corta con un
/// error: se le responde con <c>showValidationError</c>, que pinta el motivo junto al campo y deja corregirlo.
/// </summary>
public class IdentityProviderDocumentNumberTest
{
    private const string DocumentNumberKey = "extension_17aa212c04384bec8b49961f4d1acb45_DocumentNumber";

    private static readonly DocumentType TaxId = new(Guid.NewGuid(), "Tax identification number", "31");

    private readonly Mock<IMediator> mediator = new();
    private readonly IdentityProviderController controller;

    public IdentityProviderDocumentNumberTest()
    {
        mediator.Setup(x => x.Send(It.IsAny<FindTypeDocumentByCodeQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(TaxId);

        controller = new IdentityProviderController(mediator.Object, NullLogger<IdentityProviderController>.Instance);
    }

    [Fact]
    public async Task CreateUser_CheckDigitIsMissing_ShowsTheReasonOnTheDocumentNumber()
    {
        // Arrange
        this.TheDomainRejectsWith(DomainErrors.DocumentCheckDigitIsMissing.With("901888777", 3));

        // Act
        var response = await controller.CreateUser(BuildRequest("901888777", "es-co"), CancellationToken.None);

        // Assert
        var action = Assert.IsType<ShowValidationError>(Assert.Single(response.Data.Actions));

        Assert.Equal("Corrige el dato señalado para continuar.", action.Message);
        Assert.Equal("Este tipo de documento se escribe con guion y dígito de verificación. Si 901888777 es el número sin el dígito, escríbelo así: 901888777-3.", Assert.Single(action.AttributeErrors, x => x.Key == DocumentNumberKey).Value);
    }

    [Fact]
    public async Task CreateUser_CheckDigitDoesNotMatch_SaysWhichDigitItIs()
    {
        // Arrange
        this.TheDomainRejectsWith(DomainErrors.DocumentCheckDigitDoesNotMatch.With("901888777", 3, '5'));

        // Act
        var response = await controller.CreateUser(BuildRequest("901888777-5", "es-co"), CancellationToken.None);

        // Assert
        var action = Assert.IsType<ShowValidationError>(Assert.Single(response.Data.Actions));

        Assert.Equal("El dígito de verificación no corresponde: el de 901888777 es 3, no 5. Escríbelo así: 901888777-3.", action.AttributeErrors[DocumentNumberKey]);
    }

    [Fact]
    public async Task CreateUser_CheckDigitIsNotSeparated_ProposesTheNumberWithItsDigit()
    {
        // Arrange
        this.TheDomainRejectsWith(DomainErrors.DocumentCheckDigitIsNotSeparated.With("901888777", '3'));

        // Act
        var response = await controller.CreateUser(BuildRequest("9018887773", "es-co"), CancellationToken.None);

        // Assert
        var action = Assert.IsType<ShowValidationError>(Assert.Single(response.Data.Actions));

        Assert.Equal("Este tipo de documento se escribe con guion y dígito de verificación. Escríbelo así: 901888777-3.", action.AttributeErrors[DocumentNumberKey]);
    }

    [Fact]
    public async Task CreateUser_BaseLengthIsInvalid_SaysHowManyDigitsGoBeforeTheDash()
    {
        // Arrange
        this.TheDomainRejectsWith(DomainErrors.DocumentBaseLengthIsInvalid.With(9, 10));

        // Act
        var response = await controller.CreateUser(BuildRequest("9018887773-5", "es-co"), CancellationToken.None);

        // Assert
        var action = Assert.IsType<ShowValidationError>(Assert.Single(response.Data.Actions));

        Assert.Equal("Este tipo de documento lleva 9 dígitos antes del guion, y el número que escribiste tiene 10.", action.AttributeErrors[DocumentNumberKey]);
    }

    [Fact]
    public async Task CreateUser_NumberFormatIsInvalid_ShowsTheReasonOnTheDocumentNumber()
    {
        // Arrange
        this.TheDomainRejectsWith(DomainErrors.DocumentNumberFormatIsInvalid);

        // Act
        var response = await controller.CreateUser(BuildRequest("901.888.777-3", "es-co"), CancellationToken.None);

        // Assert
        var action = Assert.IsType<ShowValidationError>(Assert.Single(response.Data.Actions));

        Assert.StartsWith("Este tipo de documento se escribe solo con dígitos", action.AttributeErrors[DocumentNumberKey]);
    }

    [Theory]
    [InlineData("en-us", "Fix the highlighted field to continue.")]
    [InlineData("pt-br", "Corrija o campo indicado para continuar.")]
    [InlineData("fr-fr", "Corrigez le champ signalé pour continuer.")]
    [InlineData(null, "Fix the highlighted field to continue.")]
    public async Task CreateUser_DocumentNumberRejected_AnswersInTheLanguageOfTheForm(string? locale, string expected)
    {
        // Arrange
        this.TheDomainRejectsWith(DomainErrors.DocumentCheckDigitIsMissing.With("901888777", 3));

        // Act
        var response = await controller.CreateUser(BuildRequest("901888777", locale), CancellationToken.None);

        // Assert
        var action = Assert.IsType<ShowValidationError>(Assert.Single(response.Data.Actions));

        Assert.Equal(expected, action.Message);
        Assert.Contains("901888777-3", action.AttributeErrors[DocumentNumberKey]);
    }

    [Fact]
    public async Task CreateUser_DocumentNumberRejected_SerializesAsEntraExpects()
    {
        // Arrange
        this.TheDomainRejectsWith(DomainErrors.DocumentCheckDigitIsMissing.With("901888777", 3));

        // Act
        var response = await controller.CreateUser(BuildRequest("901888777", "en-us"), CancellationToken.None);

        // Assert
        var json = JObject.Parse(JsonConvert.SerializeObject(response));
        var action = Assert.Single(json["data"]!["actions"]!);

        Assert.Equal("microsoft.graph.onAttributeCollectionSubmitResponseData", (string?)json["data"]!["@odata.type"]);
        Assert.Equal("microsoft.graph.attributeCollectionSubmit.showValidationError", (string?)action["@odata.type"]);
        Assert.Equal("Fix the highlighted field to continue.", (string?)action["message"]);
        Assert.Contains("901888777-3", (string?)action["attributeErrors"]![DocumentNumberKey]);
    }

    [Fact]
    public async Task CreateUser_ValidDocumentNumber_SerializesTheDefaultBehavior()
    {
        // Act
        var response = await controller.CreateUser(BuildRequest("901888777-3", "es-co"), CancellationToken.None);

        // Assert
        var json = JObject.Parse(JsonConvert.SerializeObject(response));
        var action = Assert.Single(json["data"]!["actions"]!);

        Assert.Equal("microsoft.graph.attributeCollectionSubmit.continueWithDefaultBehavior", (string?)action["@odata.type"]);
        Assert.Single(action.Children());
    }

    [Fact]
    public async Task CreateUser_AnotherDomainRejection_IsNotTurnedIntoAValidationError()
    {
        // Arrange
        this.TheDomainRejectsWith(DomainErrors.PhoneIsRequired);

        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => controller.CreateUser(BuildRequest("901888777-3", "es-co"), CancellationToken.None));

        // Assert
        Assert.Equal(DomainErrors.PhoneIsRequired.GetCode(), exception.Code);
    }

    [Fact]
    public async Task CreateUser_SameCodeFromAnotherLayer_IsNotTurnedIntoAValidationError()
    {
        // Arrange
        mediator.Setup(x => x.Send(It.IsAny<CreateUserFromSSOCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new CodeDesignPlusException(Layer.Application, DomainErrors.DocumentCheckDigitIsMissing.With("901888777", 3)));

        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => controller.CreateUser(BuildRequest("901888777", "es-co"), CancellationToken.None));

        // Assert
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public void Request_FromEntra_BindsTheLocaleOfTheForm()
    {
        // Arrange
        const string payload = """
            {
              "type": "microsoft.graph.authenticationEvent.attributeCollectionSubmit",
              "data": {
                "@odata.type": "microsoft.graph.onAttributeCollectionSubmitCalloutData",
                "authenticationContext": { "correlationId": "c", "client": { "ip": "30.51.176.110", "locale": "es-co", "market": "es-co" } },
                "userSignUpInfo": { "attributes": { "givenName": { "@odata.type": "microsoft.graph.stringDirectoryAttributeValue", "value": "Joe", "attributeType": "builtIn" } }, "identities": [] }
              }
            }
            """;

        // Act
        var request = JsonConvert.DeserializeObject<OnAttributeCollectionSubmitRequest>(payload);

        // Assert
        Assert.Equal("es-co", request!.Data.AuthenticationContext!.Client!.Locale);
    }

    private void TheDomainRejectsWith(Error error)
    {
        mediator.Setup(x => x.Send(It.IsAny<CreateUserFromSSOCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new CodeDesignPlusException(Layer.Domain, error));
    }

    private static OnAttributeCollectionSubmitRequest BuildRequest(string documentNumber, string? locale)
    {
        return new OnAttributeCollectionSubmitRequest
        {
            Data = new OnAttributeCollectionSubmitData
            {
                AuthenticationContext = locale is null ? null : new SignUpAuthenticationContext { Client = new SignUpClient { Locale = locale } },
                UserSignUpInfo = new UserSignUpInfo
                {
                    Attributes = new Dictionary<string, UserSignUpInfoAttribute>
                    {
                        ["givenName"] = new() { Value = "Joe" },
                        ["surname"] = new() { Value = "Doe" },
                        ["extension_17aa212c04384bec8b49961f4d1acb45_Phone"] = new() { Value = "+573105550131" },
                        [DocumentNumberKey] = new() { Value = documentNumber },
                        ["extension_17aa212c04384bec8b49961f4d1acb45_DocumentType"] = new() { Value = "31" },
                    },
                    Identities = [new Identity { SignInType = "emailAddress", IssuerAssignedId = "joe@fake.com" }],
                },
            },
        };
    }
}
