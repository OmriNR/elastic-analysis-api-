using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountsService _discountsService;

    public DiscountsController(IDiscountsService discountsService)
    {
        _discountsService = discountsService;
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        try
        {
            var discount = _discountsService.GetDiscountById(id, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                default:
                    return Ok(discount);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("productId/{id}")]
    public IActionResult GetProductId(string id)
    {
        try
        {
            var discount = _discountsService.GetDiscountByProduct(id, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                default:
                    return Ok(discount);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("all")]
    public IActionResult GetAll()
    {
        try
        {
            var discounts = _discountsService.GetAllActiveDiscounts();
            return Ok(discounts);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    public IActionResult Post([FromBody] Discount discount)
    {
        try
        {
            var newDiscount = _discountsService.CreateDiscount(discount, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok(newDiscount);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("category/{category}")]
    public IActionResult CreateForCategory(string category, [FromBody] Discount discount)
    {
        var isAdmin = User.FindFirst("is_admin")?.Value == "true";
        if (!isAdmin)
            return Forbid();

        try
        {
            var newDiscounts = _discountsService.CreateDiscountsByCategory(discount, category, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok(newDiscounts);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("user/{userId}")]
    public IActionResult CreateForUser(string userId, [FromBody] Discount discount)
    {
        var requestingUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        var isAdmin = User.FindFirst("is_admin")?.Value == "true";

        if (!isAdmin && requestingUserId != userId)
            return Forbid();

        try
        {
            var newDiscounts = _discountsService.CreateDiscountsByUser(discount, userId, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok(newDiscounts);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var isAdmin = User.FindFirst("is_admin")?.Value == "true";
        if (!isAdmin)
            return Forbid();

        try
        {
            _discountsService.DeleteDiscount(id, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                default:
                    return Ok();
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}
