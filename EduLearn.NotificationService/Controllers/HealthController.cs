using Microsoft.AspNetCore.Mvc;

namespace EduLearn.NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get(CancellationToken cancellationToken)
    {
        return Ok(new { status = "healthy", service = "NotificationService", timestamp = DateTime.UtcNow });
    }
}
