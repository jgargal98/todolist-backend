using Microsoft.AspNetCore.Mvc;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/helloworld
public class HelloWorldController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Si lees esto me debes 20 pavos");
    }
}