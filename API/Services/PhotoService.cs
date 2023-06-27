using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Imagekit.Sdk;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly ImagekitClient _imagekitClient;
        public PhotoService(IOptions<ImageKitSettings> imageKitSettings)
        {
            _imagekitClient = new ImagekitClient(
                publicKey: imageKitSettings.Value.ApiKey,
                privateKey: imageKitSettings.Value.ApiSecret,
                urlEndPoint: imageKitSettings.Value.ImageKitID
                );

        }
        public ResultDelete DeletePhoto(string photoId)
        {
            ResultDelete res2 = _imagekitClient.DeleteFile(photoId);
            return res2;
        }

        public async Task<Result> UploadPhoto(IFormFile file)
        {
            var bytes = await file.GetBytes();

            FileCreateRequest request = new FileCreateRequest
            {
                file = bytes,
                fileName = file.FileName,
            };

            return await _imagekitClient.UploadAsync(request);
        }
    }
}
