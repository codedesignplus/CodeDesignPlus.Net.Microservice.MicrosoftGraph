using CodeDesignPlus.Net.Exceptions.Guards;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CompleteProviderRegistration;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUserFromSSO;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.TypeDocument.Queries.FindByCode;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Queries.GetByIdentityProviderId;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.Controllers;

/// <summary>
/// Provides API endpoints for Microsoft Entra External ID user flows (CIAM).
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class IdentityProviderController(IMediator mediator, ILogger<IdentityProviderController> logger) : ControllerBase
{
    /// <summary>
    /// Receives user attributes from an Entra External ID sign-up flow and creates the user aggregate synchronously.
    /// </summary>
    /// <remarks>
    /// El tipo de documento llega como el codigo que eligio la persona en la lista desplegable del registro (atributo
    /// <c>extension_..._DocumentType</c>; en Colombia, el codigo DIAN) y se resuelve contra los tipos sembrados del pais,
    /// los mismos de ms-catalogs: la factura electronica lleva el tipo y el numero del adquiriente y la DIAN no admite uno
    /// por defecto (pendings/663).
    /// </remarks>
    [HttpPost("OnAttributeCollectionSubmit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<OnAttributeCollectionSubmitResponse> CreateUser([FromBody] OnAttributeCollectionSubmitRequest request, CancellationToken cancellationToken)
    {
        InfrastructureGuard.IsNull(request.Data.UserSignUpInfo!, Errors.InvalidSignUpInfo);

        var displayName = request.Data.UserSignUpInfo.Attributes.FirstOrDefault(x => x.Key == "displayName").Value?.Value;
        var givenName = request.Data.UserSignUpInfo.Attributes.FirstOrDefault(x => x.Key == "givenName").Value?.Value;
        var surname = request.Data.UserSignUpInfo.Attributes.FirstOrDefault(x => x.Key == "surname").Value?.Value;
        var phone = request.Data.UserSignUpInfo.Attributes.FirstOrDefault(x => x.Key.Contains("phone", StringComparison.OrdinalIgnoreCase)).Value?.Value;
        var email = request.Data.UserSignUpInfo.Identities.FirstOrDefault(x => x.SignInType == "emailAddress")?.IssuerAssignedId;
        var documentNumber = request.Data.UserSignUpInfo.Attributes.FirstOrDefault(x => x.Key.Contains("documentNumber", StringComparison.OrdinalIgnoreCase)).Value?.Value;
        var documentTypeCode = request.Data.UserSignUpInfo.Attributes.FirstOrDefault(x => x.Key.Contains("documentType", StringComparison.OrdinalIgnoreCase)).Value?.Value;

        if (string.IsNullOrEmpty(displayName))
            displayName = $"{givenName} {surname}";

        InfrastructureGuard.IsNullOrEmpty(email!, Errors.EmailIsRequired);
        InfrastructureGuard.IsNullOrEmpty(givenName!, Errors.GivenNameIsRequired);
        InfrastructureGuard.IsNullOrEmpty(surname!, Errors.SurnameIsRequired);
        InfrastructureGuard.IsNullOrEmpty(phone!, Errors.PhoneIsRequired);
        InfrastructureGuard.IsNullOrEmpty(documentNumber!, Errors.DocumentNumberIsRequired);
        InfrastructureGuard.IsNullOrWhiteSpace(documentTypeCode!, Errors.DocumentTypeIsRequired);

        var documentType = await mediator.Send(new FindTypeDocumentByCodeQuery(documentTypeCode!), cancellationToken);

        InfrastructureGuard.IsNull(documentType!, Errors.DocumentTypeNotFound);

        await mediator.Send(new CreateUserFromSSOCommand(givenName!, surname!, email!, phone!, displayName!, documentNumber!, documentType!, true), cancellationToken);

        var response = OnAttributeCollectionSubmitResponse.Create();

        response.Data.Actions.Add(ContinueWithDefaultBehavior.Create());

        return response;
    }

    /// <summary>
    /// Handles the custom claims provider token issuance event. Completes the provider registration
    /// with the IdentityProviderId from Entra and emits UserCreatedDomainEvent for downstream consumers.
    /// </summary>
    [HttpPost("TokenIssuance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> TokenIssuance([FromBody] TokenIssuanceRequest request, CancellationToken cancellationToken)
    {
        var correlationId = request.Data.AuthenticationContext.CorrelationId;
        var entraUserId = request.Data.AuthenticationContext.User.Id;
        var mail = request.Data.AuthenticationContext.User.Mail;
        var upn = request.Data.AuthenticationContext.User.UserPrincipalName;
        var displayName = request.Data.AuthenticationContext.User.DisplayName;

        logger.LogInformation(
            "TokenIssuance started. CorrelationId: {CorrelationId} | EntraUserId: {EntraUserId} | Mail: {Mail} | UPN: {UPN} | DisplayName: {DisplayName}",
            correlationId, entraUserId, mail, upn, displayName);

        var email = mail;

        if (string.IsNullOrEmpty(email))
        {
            logger.LogDebug("TokenIssuance: Mail is empty, falling back to UserPrincipalName. EntraUserId: {EntraUserId} | UPN: {UPN}", entraUserId, upn);
            email = upn;
        }

        logger.LogDebug("TokenIssuance: resolved email = {Email}. Sending CompleteProviderRegistrationCommand.", email);

        var userId = await mediator.Send(new CompleteProviderRegistrationCommand(email, entraUserId), cancellationToken);

        logger.LogInformation(
            "TokenIssuance completed successfully. CorrelationId: {CorrelationId} | EntraUserId: {EntraUserId} | ResolvedUserId: {UserId}",
            correlationId, entraUserId, userId);

        var response = TokenIssuanceResponse.Create();

        response.Data.Actions.Add(ActionProviderClaim.Create("userId", userId.ToString()));

        return Ok(response);
    }
}