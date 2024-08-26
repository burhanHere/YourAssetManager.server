namespace YourAssetManager.Server.Services
{
    public interface ICloudinaryService
    {
        public Task<string> UploadImageToCloudinaryAsync(Stream imageStream, string fileName);
    }
}