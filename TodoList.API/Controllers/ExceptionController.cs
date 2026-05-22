using Microsoft.AspNetCore.Mvc;

namespace TodoList.API.Controllers;

/// <summary>
/// Controller used for testing the global exception handling middleware.
/// </summary>
[ApiController]
[Route("api/[controller]")] // /api/Exception
public class ExceptionController : ControllerBase
{
    /// <summary>Throws an exception to verify the middleware catches it correctly.</summary>
    [HttpGet]
    public IActionResult Get()
    {
        throw new Exception("Error handler test");
    }
}