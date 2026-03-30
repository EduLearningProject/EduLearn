using Microsoft.AspNetCore.Mvc;

namespace EduLearn.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get(CancellationToken cancellationToken)
    {
        return Ok(new { status = "healthy", service = "AuthService", timestamp = DateTime.UtcNow });
    }
}
