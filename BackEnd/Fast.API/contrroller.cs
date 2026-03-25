using Microsoft.AspNetCore.Mvc;

namespace Fast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return "API çalışıyor aga 😎";
    }
}