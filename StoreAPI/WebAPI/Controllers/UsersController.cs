using Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersPropertiesService _usersService;

    public UsersController(IUsersPropertiesService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet("{userId}")]
    public IActionResult GetUser(string userId)
    {
        try
        {
            var user = _usersService.GetUser(userId, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                default:
                    return Ok(user);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult PostUser([FromBody] UserProperties user)
    {
        try
        {
            var newUser = _usersService.CreateUser(user, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok(newUser);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPut]
    public IActionResult PutUser([FromBody] UserProperties user)
    {
        try
        {
            var updated = _usersService.UpdeateUser(user, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok(updated);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(string userId)
    {
        try
        {
            _usersService.DeleteUser(userId, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok($"Item {userId} was deleted");
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}