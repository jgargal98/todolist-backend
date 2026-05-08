using Microsoft.AspNetCore.Mvc;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/testdb
public class TestDBController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { mensaje = "Si lees esto me debes 20 pavos" });
    }
}