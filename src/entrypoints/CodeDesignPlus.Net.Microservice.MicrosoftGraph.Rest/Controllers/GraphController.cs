namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Rest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GraphController() : ControllerBase
{
    /// <summary>
    /// Hello World endpoint for testing purposes.
    /// </summary>
    /// <returns>A simple "Hello World" message.</returns>
    [HttpGet]
    public Task<IActionResult> HelloWorld()
    {
        var result = new { message = "Hello World" };
        return Task.FromResult<IActionResult>(Ok(result));
    }
}
