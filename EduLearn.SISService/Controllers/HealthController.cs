using Microsoft.AspNetCore.Mvc;

namespace EduLearn.SISService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get(CancellationToken cancellationToken)
    {
        return Ok(new { status = "healthy", service = "SISService", timestamp = DateTime.UtcNow });
    }
}
