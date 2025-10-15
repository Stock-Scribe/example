using Microsoft.AspNetCore.Mvc;

namespace Nexon.FleaMarket.Api.Controllers;


[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { ok = true, time = DateTime.UtcNow });
}