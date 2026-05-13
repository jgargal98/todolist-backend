using Microsoft.AspNetCore.Mvc;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/Exception
public class ExceptionController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        throw new Exception("Error handler test");
    }
}