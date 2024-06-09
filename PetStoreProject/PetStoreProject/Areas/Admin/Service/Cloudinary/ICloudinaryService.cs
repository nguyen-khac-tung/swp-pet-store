using CloudinaryDotNet.Actions;

namespace PetStoreProject.Areas.Admin.Service.Cloudinary
{
    public interface ICloudinaryService
    {
        public Task<ImageUploadResult> UploadImage(string imageData, string imageId);
    }
}
