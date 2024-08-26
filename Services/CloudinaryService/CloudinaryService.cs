
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace YourAssetManager.Server.Services
{
    public class CloudinaryService(IConfiguration configuration) : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary = new(new Account(
               cloud: configuration["Cloudinary:CloudName"]!,
               apiKey: configuration["Cloudinary:ApiKey"]!,
               apiSecret: configuration["Cloudinary:ApiSecret"]!
           ));
        public async Task<string> UploadImageToCloudinaryAsync(Stream imageStream, string fileName)
        {
            var uplaodParameters = new ImageUploadParams
            {
                File = new FileDescription(fileName, imageStream),
                AssetFolder = "Test"
            };
            ImageUploadResult uploadResult = await _cloudinary.UploadAsync(uplaodParameters);
            return uploadResult.SecureUrl.AbsoluteUri;
        }
    }
}
