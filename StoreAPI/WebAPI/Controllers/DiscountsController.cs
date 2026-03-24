using Domain;
using Microsoft.AspNetCore.Http.HttpResults;
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
    
    [HttpPost("category/{category}")]
    public IActionResult CraeteForCategory(string category, [FromBody] Discount discount)
    {
        try
        {
            var newDiscount = _discountsService.CreateDiscountByCategory(discount, category, out var status, out var error);

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

    [HttpPost("user/{userId}")]
    public IActionResult CreateForUser(string userId, [FromBody] Discount discount)
    {
        try
        {
            var newDiscount = _discountsService.CreateDiscountByUser(discount, userId, out var status, out var error);

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
}