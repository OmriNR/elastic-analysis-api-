using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
public class PublisherController : ControllerBase
{
    [HttpGet("TimeUntilPublish")]
    public IActionResult GetTimeUntilPublish()
    {
        return Ok();
    }
}