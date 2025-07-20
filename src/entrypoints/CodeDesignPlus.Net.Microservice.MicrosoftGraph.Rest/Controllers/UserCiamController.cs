using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Exceptions.Guards;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.Controllers;

/// <summary>
/// Provides API endpoints for Microsoft Entra External ID user flows (CIAM).
/// </summary>
/// <remarks>
/// This controller is specifically designed to handle webhooks from custom authentication extensions,
/// such as the OnAttributeCollectionSubmit event, to process user data during sign-up.
/// SECURITY: Endpoints in this controller should be protected, for instance, using an API Key
/// that is configured in both the API and the Microsoft Entra custom extension settings.
/// </remarks>
/// <param name="mediator">The MediatR instance for dispatching commands.</param>
/// <param name="mapper">The AutoMapper instance for object-to-object mapping.</param>
[Route("api/[controller]")]
[ApiController]
public class UserCiamController(IMediator mediator, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Receives user attributes from an Entra External ID flow to temporarily create a user.
    /// </summary>
    /// <remarks>
    /// This endpoint is intended to be called by a Microsoft Entra custom authentication extension
    /// during the 'OnAttributeCollectionSubmit' step of a user sign-up flow.
    /// It processes the user data and returns a structured response to Entra to either continue the flow
    /// or block it with a validation message.
    /// </remarks>
    /// <param name="request">The payload sent by Microsoft Entra, containing the collected user attributes.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <response code="200">The user data was processed successfully, and the user flow should continue.</response>
    /// <response code="400">If the submitted data is invalid. The response body contains a structured error message for Entra to display to the user.</response>
    /// <response code="401">If the request is not properly authenticated (e.g., missing or invalid API Key).</response>
    /// <response code="500">If an unexpected internal server error occurs.</response>
    [HttpPost("OnAttributeCollectionSubmit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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

        if (string.IsNullOrEmpty(displayName))
            displayName = $"{givenName} {surname}";

        InfrastructureGuard.IsNullOrEmpty(email!, Errors.EmailIsRequired);
        InfrastructureGuard.IsNullOrEmpty(givenName!, Errors.GivenNameIsRequired);
        InfrastructureGuard.IsNullOrEmpty(surname!, Errors.SurnameIsRequired);
        InfrastructureGuard.IsNullOrEmpty(phone!, Errors.PhoneIsRequired);

        await mediator.Send(new CreateUserCiamCommand(givenName!, surname!, email!, phone!, displayName!, true), cancellationToken);

        var actions = new List<ContinueWithDefaultBehavior>
        {
            new() { Type = "microsoft.graph.attributeCollectionSubmit.continueWithDefaultBehavior"}
        };

        var response = new OnAttributeCollectionSubmitResponse
        {
            Data = new Data
            {
                Type = "microsoft.graph.onAttributeCollectionSubmitResponseData",
                Actions = actions
            }
        };

        return response;
    }

}