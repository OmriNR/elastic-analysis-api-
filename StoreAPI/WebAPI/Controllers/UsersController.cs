using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using WebAPI.DTOs;
using Services.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _service;
    private readonly TokenService _tokenService;

    public UsersController(IUsersService service, TokenService tokenService)
    {
        _service = service;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
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
                    return Ok(new LoginResponse(user, _tokenService.GenerateToken(user)));
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("create")]
    public IActionResult CreateUser([FromBody] User user)
    {
        try
        {
            var newUser = _service.CreateUser(user, out var status, out var error);

            switch (status)
            {
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok(new LoginResponse(newUser, _tokenService.GenerateToken(newUser)));
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [Authorize]
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
