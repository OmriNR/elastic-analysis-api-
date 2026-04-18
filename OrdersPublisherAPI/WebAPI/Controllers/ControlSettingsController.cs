using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ControlSettingsController : ControllerBase
{
    private readonly WorkerControlService _service;

    public ControlSettingsController(WorkerControlService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult TurnOnOffWorker()
    {
        _service.IsEnabled = !_service.IsEnabled;
        return Ok();
    }

    [HttpPost]
    public IActionResult SetWorkerTime(string time)
    {
        if (TimeSpan.TryParse(time, out var newTime))
        {
            _service.ExecutionTimeout = newTime;
            return Ok();
        }
        return BadRequest("Invalid time format");
    }
}