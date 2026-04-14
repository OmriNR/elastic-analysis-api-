using Domain;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services;

public class ImagesService : IImagesService
{
    private readonly IImagesRepository _imagesRepository;
    private readonly IProductsRepository _productsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly ILogger<ImagesService> _logger;
    
    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

    public ImagesService(IImagesRepository imagesRepository, IProductsRepository productsRepository, IUsersRepository usersRepository, ILogger<ImagesService> logger)
    {
        _imagesRepository = imagesRepository;
        _productsRepository = productsRepository;
        _usersRepository = usersRepository;
        _logger = logger;
    }

    public void UploadProductImage(string productId, Stream stream, string originalFileName, out Statuses status, out string error)
    {
        _logger.LogInformation($"Uploading image for product {productId}");
        status = Statuses.OK;
        error = string.Empty;
        
        var exist = _productsRepository.GetProduct(productId) != null;

        if (exist)
        {
            if (CheckFile(stream, originalFileName, out string contentType, out error))
            {
                _imagesRepository.UploadImage(productId, stream, contentType).Wait();
            }
            else
            {
                _logger.LogError(error);
                status = Statuses.INVALID;
                return;
            }
        }
        else
        {
            _logger.LogError($"Product {productId} not found");
            status = Statuses.NOT_FOUND;
            error = $"Product {productId} not found";
            return;
        }
    }

    public void UploadUserImage(string userId, Stream stream, string originalFileName, out Statuses status, out string error)
    {
        _logger.LogInformation($"Uploading image for user {userId}");
        status = Statuses.OK;
        error = string.Empty;
        
        var exist = _usersRepository.GetUserById(userId) != null;

        if (exist)
        {
            if (CheckFile(stream, originalFileName, out string contentType, out error))
            {
                _imagesRepository.UploadImage(userId, stream, contentType).Wait();
            }
            else
            {
                _logger.LogError(error);
                status = Statuses.INVALID;
                return;
            }
        }
        else
        {
            _logger.LogError($"User {userId} not found");
            status = Statuses.NOT_FOUND;
            error = $"User {userId} not found";
            return;
        }
    }

    public Stream GetProductImage(string productId, out Statuses status, out string error)
    {
        _logger.LogInformation($"Getting product {productId} image");
        status = Statuses.OK;
        error = string.Empty;
        
        var exist =  _productsRepository.GetProduct(productId) != null;

        if (exist)
        {
            var image = _imagesRepository.GetImage(productId);
            image.Wait();
            return image.Result;
        }
        else
        {
            _logger.LogError($"Product {productId} not found");
            status = Statuses.NOT_FOUND;
            error = $"Product {productId} not found";
            return null;
        }
    }

    public Stream GetUserImage(string userId, out Statuses status, out string error)
    {
        _logger.LogInformation($"Getting user {userId} image");
        status = Statuses.OK;
        error = string.Empty;
        
        var exist =  _usersRepository.GetUserById(userId) != null;

        if (exist)
        {
            var image = _imagesRepository.GetImage(userId);
            image.Wait();
            return image.Result;
        }
        else
        {
            _logger.LogError($"User {userId} not found");
            status = Statuses.NOT_FOUND;
            error = $"User {userId} not found";
            return null;
        }
    }

    private bool CheckFile(Stream fileStream, string fileName, out string contentType, out string error)
    {
        error = string.Empty;
        contentType = string.Empty;
        
        string extension = Path.GetExtension(fileName).ToLowerInvariant();
        //5MB
        if (fileStream.Length > 5 * 1024 * 1024)
        {
            error = "File is too big, only below 5MB";
            return false;
        }

        if (!allowedExtensions.Contains(extension))
        {
            error = "Invalid file extension";
            return false;
        }

        contentType = extension switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };
        
        return true;
    }
}