using Microsoft.AspNetCore.Mvc;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/testdb
public class TestDBController : ControllerBase
{
    // Este método responde a GET /api/todo
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { mensaje = "Si lees esto me debes 20 pavos" });
    }
}