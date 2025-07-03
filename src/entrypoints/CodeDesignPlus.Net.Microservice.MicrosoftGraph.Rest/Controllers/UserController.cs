namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.Controllers;

/// <summary>
/// Controller for managing Azure AD users.
/// </summary>
/// <param name="mediator">Mediator instance for sending commands and queries.</param>
/// <param name="mapper">Mapper instance for mapping between DTOs and commands/queries.</param>
[Route("api/[controller]")]
[ApiController]
public class UserController(IMediator mediator, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Create a new user.
    /// </summary>
    /// <param name="data">Data for creating the Graph.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto data, CancellationToken cancellationToken)
    {
        data.IsActive = true; 
        await mediator.Send(mapper.Map<CreateUserCommand>(data), cancellationToken);

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
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdateProfileCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Update the identity information of an existing user in Azure AD.
    /// </summary>
    /// <param name="id">The unique identifier of the identity.</param>
    /// <param name="data">Data for updating the Graph.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPatch("{id}/identity")]
    public async Task<IActionResult> UpdateIdentity(Guid id, [FromBody] UpdateIdentityDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdateIdentityCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Update the contact information of an existing user in Azure AD.
    /// </summary>
    /// <param name="id">The unique identifier of the identity.</param>
    /// <param name="data">Data for updating the Graph.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPatch("{id}/contact")]
    public async Task<IActionResult> UpdateContact(Guid id, [FromBody] UpdateContactInfoDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdateContactInfoCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Update the job information of an existing user in Azure AD.
    /// </summary>
    /// <param name="id">The unique identifier of the identity.</param>
    /// <param name="data">Data for updating the Graph.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPatch("{id}/job")]
    public async Task<IActionResult> UpdateJob(Guid id, [FromBody] UpdateJobDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdateJobCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete an existing user in Azure AD.
    /// </summary>
    /// <param name="id">The unique identifier of the identity.</param>
    /// <param name="data">Data for updating the Graph.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id, [FromBody] DeleteUserDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<DeleteUserCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Assign a group to an existing user in Azure AD.
    /// </summary>
    /// <param name="id">The unique identifier of the identity.</param>
    /// <param name="data">Data for updating the Graph.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPost("{id}/group")]
    public async Task<IActionResult> AddGroup(Guid id, [FromBody] AddGroupToUserDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<AddGroupToUserCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///  Remove a group from an existing user in Azure AD.
    /// </summary>
    /// <param name="id">The unique identifier of the identity.</param>
    /// <param name="data">Data for updating the Graph.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpDelete("{id}/group")]
    public async Task<IActionResult> RemoveGroup(Guid id, [FromBody] RemoveGroupToUserDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<RemoveGroupToUserCommand>(data), cancellationToken);

        return NoContent();
    }
}