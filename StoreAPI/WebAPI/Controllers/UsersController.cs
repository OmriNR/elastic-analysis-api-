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
    [HttpGet("{id}")]
    public IActionResult GetUserById(string id)
    {
        try
        {
            var user = _service.GetUserById(id);
            if (user == null) return NotFound("User not found");
            return Ok(new PublicUserDto(user.UserId, user.Properties.UserName, user.CreatedAt.ToString("o")));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
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

            if (user.IsAdmin)
            {
                return BadRequest("This endpoint is not used for admin permissions");
            }
            
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

    [Authorize]
    [HttpPost("setAdmin")]
    public IActionResult SetAdmin([FromBody] UserRequest userRequest)
    {
        try
        {
            _service.SetAdmin(userRequest.TargetUserId, userRequest.RequestedUserId, out var status, out var error);

            if (status == Statuses.NOT_FOUND)
                return NotFound(error);

            if (status == Statuses.INVALID)
                return StatusCode(403, error);

            return Ok($"User {userRequest.TargetUserId} has became admin by {userRequest.RequestedUserId}");
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("setActivity")]
    public IActionResult SetActivity([FromBody] UserRequest userRequest)
    {
        try
        {
            _service.SetActive(userRequest.TargetUserId, userRequest.RequestedUserId, out var status, out var error);

            if (status == Statuses.NOT_FOUND)
                return NotFound(error);

            if (status == Statuses.INVALID)
                return StatusCode(403, error);

            return Ok($"User {userRequest.TargetUserId} activity has been changed by {userRequest.RequestedUserId}");
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [Authorize]
    [HttpGet("getAllUsers({userId}")]
    public IActionResult GetAllUsers(string userId)
    {
        try
        {
            var allUsers = _service.GetAllUsers(userId, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return StatusCode(403, error);
                default:
                    return Ok(allUsers);
            }
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
}
