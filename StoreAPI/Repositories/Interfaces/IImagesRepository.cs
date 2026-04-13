namespace Repositories.Interfaces;

public interface IImagesRepository
{
    Task UploadImage(string entityId, Stream fileStream, string contentType);
    Task<Stream> GetImage(string entityId);
}