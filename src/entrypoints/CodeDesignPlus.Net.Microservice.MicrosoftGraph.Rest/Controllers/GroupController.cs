using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.Controllers;


/// <summary>
/// Controller for managing Azure AD Groups.
/// </summary>
/// <param name="mediator">Mediator instance for sending commands and queries.</param>
/// <param name="mapper">Mapper instance for mapping between DTOs and commands/queries.</param>
[Route("api/[controller]")]
[ApiController]
public class GroupController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Temp()
    {
        var user = HttpContext.RequestServices.GetRequiredService<IUserContext>();
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<GroupController>>();

        var accessToken = user.AccessToken;
        logger.LogWarning("Access Token: {AccessToken}", accessToken);
        logger.LogWarning("Oid: {UserId}, User ID: {UserId}, User Email: {@Emails}", user.Oid, user.IdUser, user.Emails);

        return Ok(user);
    }

    /// <summary>
    /// Create a new Group.
    /// </summary>
    /// <param name="data">Data for creating the Graph.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto data, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<CreateGroupCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Update an existing profile in Azure AD.
    /// </summary>
    /// <param name="id">The unique identifier of the identity.</param>
    /// <param name="data">Data for updating the Graph.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGroup(Guid id, [FromBody] UpdateProfileDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdateProfileCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete an existing Group in Azure AD.
    /// </summary>
    /// <param name="id">The unique identifier of the identity.</param>
    /// <param name="data">Data for updating the Graph.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(Guid id, [FromBody] DeleteGroupDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<DeleteGroupCommand>(data), cancellationToken);

        return NoContent();
    }
}