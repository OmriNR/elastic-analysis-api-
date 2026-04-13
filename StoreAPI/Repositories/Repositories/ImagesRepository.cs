using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Repositories.Interfaces;

namespace Repositories.Repositories;

public class ImagesRepository : IImagesRepository
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName = "store-images";

    public ImagesRepository(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }


    public async Task UploadImage(string entityId, Stream fileStream, string contentType)
    {
        var fileKey = $"{entityId}.webp";

        var uploadRequest = new TransferUtilityUploadRequest()
        {
            InputStream = fileStream,
            Key = fileKey,
            BucketName = _bucketName,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };
        
        var fileTransferUtility = new TransferUtility(_s3Client);
        
        await fileTransferUtility.UploadAsync(uploadRequest);
    }

    public async Task<Stream> GetImage(string entityId)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{entityId}.webp"
            };
            
            var response = await _s3Client.GetObjectAsync(request);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}