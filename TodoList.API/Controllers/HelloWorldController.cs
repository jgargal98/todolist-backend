using Microsoft.AspNetCore.Mvc;

namespace TodoList.API.Controllers;

/// <summary>
/// Basic controller to verify the API connectivity and operational status.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HelloWorldController : ControllerBase
{
    /// <summary>
    /// Returns a simple greeting to confirm the service is up and running.
    /// </summary>
    /// <returns>A string message wrapped in an HTTP 200 OK response.</returns>
    /// <response code="200">Returns the connection success message.</response>
    [HttpGet]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult Get()
    {
        return Ok("hello from the api");
    }
}