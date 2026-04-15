using Domain;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebAPI.Controllers;

[ApiController]
public class ImagesController : ControllerBase
{
    private readonly IImagesService _imagesService;
    public ImagesController(IImagesService imagesService)
    {
        _imagesService = imagesService;
    }

    [HttpGet("GetProductImage({id})")]
    public IActionResult GetProductImage(string id)
    {
        try
        {
            var stream = _imagesService.GetProductImage(id, out Statuses status, out string error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return  BadRequest(error);
                default:
                    return Ok(stream);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("GetUserImage({userId})")]
    public IActionResult GetUserImage(string userId)
    {
        try
        {
            var stream = _imagesService.GetUserImage(userId, out Statuses status, out string error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return  BadRequest(error);
                default:
                    return Ok(stream);
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost("UploadUserImage/{userId}")]
    [Consumes("multipart/form-data")]
    public IActionResult UploadUserImage(string userId, IFormFile file)
    {
        try
        {
            var stream = file.OpenReadStream();
            _imagesService.UploadUserImage(userId, stream, file.FileName, out Statuses status, out string error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return  BadRequest(error);
                default:
                    return Ok();
            }
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [HttpPost("UploadProductImage/{productId}")]
    [Consumes("multipart/form-data")]
    public IActionResult UploadProductImage(string productId, IFormFile file)
    {
        try
        {
            var stream = file.OpenReadStream();
            _imagesService.UploadProductImage(productId, stream, file.FileName, out Statuses status, out string error);

            switch (status)
            {
                case Statuses.NOT_FOUND:
                    return NotFound(error);
                case Statuses.INVALID:
                    return  BadRequest(error);
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