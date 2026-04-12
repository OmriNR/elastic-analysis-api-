using Domain;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _service;

    public UsersController(IUsersService service)
    {
        this._service = service;
    }

    [HttpPost("GetUser")]
    public IActionResult GetUser([FromBody] User request)
    {
        try
        {
            var user = _service.GetUser(request.Email, request.Password, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok(user);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost("create")]
    public IActionResult CreateUser([FromBody] User user)
    {
        try
        {
            var newUser = _service.CreateUser(user, out var status, out var error);

            switch (status)
            {
                case Statuses.INVALID:
                    return  BadRequest(error);
                default:
                    return Ok(newUser);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPut("update")]
    public IActionResult UpdateUser([FromBody] User user)
    {
        try
        {
            var udapted = _service.UpdateUser(user, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok(udapted);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}