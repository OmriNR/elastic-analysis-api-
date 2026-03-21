using Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrdersService _ordersService;

    public OrdersController(IOrdersService ordersService)
    {
        _ordersService = ordersService;
    }

    [HttpGet("{orderId}")]
    public IActionResult GetOrderById(string orderId)
    {
        try
        {
            var order = _ordersService.GetOrder(orderId, out Statuses status, out string error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                default:
                    return Ok(order);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("customer/{customerId}")]
    public IActionResult GetOrdersOfCustomer(string customerId)
    {
        try
        {
            var orders = _ordersService.GetOrdersByCustomer(customerId, out Statuses status, out string error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                default:
                    return Ok(orders);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("product/{productId}")]
    public IActionResult GetOrdersOfProduct(string productId)
    {
        try
        {
            var orders = _ordersService.GetOrdersByProduct(productId, out Statuses status, out string error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                default:
                    return Ok(orders);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [HttpPost]
    public IActionResult CreateOrder([FromBody] Order order)
    {
        try
        {
            var newOrder = _ordersService.CreateOrder(order, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok(newOrder);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}