using Domain;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RelationsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        return Ok();
    }

    [HttpGet("getRelationsByUserIdAndStatus({userId}, {status}")]
    public IActionResult GetByUserId(string userId,  RelationStatuses status)
    {
        return Ok();
    }

    [HttpGet("getRelationsByProductId({productId}, {status})")]
    public IActionResult GetByProductId(string productId, RelationStatuses status)
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult Post([FromBody] Relation relation)
    {
        return Ok();
    }

    [HttpPut("setStatus({id}, {status}")]
    public IActionResult SetStatus(string id, RelationStatuses status)
    {
        return Ok();
    }
}