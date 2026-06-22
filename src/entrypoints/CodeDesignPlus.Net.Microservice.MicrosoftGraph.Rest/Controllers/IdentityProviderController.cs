using CodeDesignPlus.Net.Exceptions.Guards;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CompleteProviderRegistration;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUserFromSSO;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Queries.GetByIdentityProviderId;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.Controllers;

/// <summary>
/// Provides API endpoints for Microsoft Entra External ID user flows (CIAM).
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class IdentityProviderController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Receives user attributes from an Entra External ID sign-up flow and creates the user aggregate synchronously.
    /// </summary>
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

        if (string.IsNullOrEmpty(displayName))
            displayName = $"{givenName} {surname}";

        InfrastructureGuard.IsNullOrEmpty(email!, Errors.EmailIsRequired);
        InfrastructureGuard.IsNullOrEmpty(givenName!, Errors.GivenNameIsRequired);
        InfrastructureGuard.IsNullOrEmpty(surname!, Errors.SurnameIsRequired);
        InfrastructureGuard.IsNullOrEmpty(phone!, Errors.PhoneIsRequired);
        InfrastructureGuard.IsNullOrEmpty(documentNumber!, Errors.DocumentNumberIsRequired);

        await mediator.Send(new CreateUserFromSSOCommand(givenName!, surname!, email!, phone!, displayName!, documentNumber!, true), cancellationToken);

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
        var entraUserId = request.Data.AuthenticationContext.User.Id;
        var email = request.Data.AuthenticationContext.User.Mail;

        if (string.IsNullOrEmpty(email))
            email = request.Data.AuthenticationContext.User.UserPrincipalName;

        var userId = await mediator.Send(new CompleteProviderRegistrationCommand(email, entraUserId), cancellationToken);

        var response = TokenIssuanceResponse.Create();

        response.Data.Actions.Add(ActionProviderClaim.Create("userId", userId.ToString()));

        return Ok(response);
    }
}