using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _productsService;

    public ProductsController(IProductsService productsService)
    {
        _productsService = productsService;
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        try
        {
            var product = _productsService.GetProductById(id, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                default:
                    return Ok(product);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        try
        {
            var products = _productsService.GetAllProducts(out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return  NotFound(error);
                default:
                    return Ok(products);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("categories/all")]
    public IActionResult GetAllCategories()
    {
        var categories = _productsService.GetAllCategories();
        
        return  Ok(categories);
    }
    
    [HttpGet("categories/{catgory}")]
    public IActionResult GetByCategory(string catgory)
    {
        try
        {
            var products = _productsService.GetProductsByCategory(catgory, out var status, out var error);
            
            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                default:
                    return Ok(products);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("users/{userId}")]
    public IActionResult GetUsers(string userId)
    {
        try
        {
            var products = _productsService.GetProductsByUser(userId,  out var status, out var error);
            
            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok(products);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    [HttpPost("multi")]
    public IActionResult GetMulti([FromBody] List<string> ids)
    {
        try
        {
            var products = _productsService.GetProducts(ids, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                default:
                    return Ok(products);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [Authorize]
    [HttpPost]
    public IActionResult Post([FromBody] Product product)
    {
        try
        {
            var newProduct = _productsService.CreateProduct(product, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok(newProduct);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [Authorize]
    [HttpPut]
    public IActionResult Put([FromBody] Product product)
    {
        try
        {
            var updated = _productsService.UpdateProduct(product, out var status, out var error);

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

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        try
        {
            _productsService.DeleteProduct(id, out var status, out var error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return BadRequest(error);
                default:
                    return Ok($"Item {id} was deleted");
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}