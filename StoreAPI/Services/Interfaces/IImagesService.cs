using Domain;

namespace Services.Interfaces;

public interface IImagesService
{
    void UploadProductImage(string productId,  Stream stream, string originalFileName, out Statuses status, out string error);
    void UploadUserImage(string userId,  Stream stream, string originalFileName, out Statuses status, out string error);
    Stream GetProductImage(string productId, out Statuses status, out string error);
    Stream GetUserImage(string userId, out Statuses status, out string error);
}