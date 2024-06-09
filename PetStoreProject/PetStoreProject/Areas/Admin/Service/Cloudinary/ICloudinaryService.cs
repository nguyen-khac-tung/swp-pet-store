namespace PetStoreProject.Areas.Admin.Service.Cloudinary
{
    public interface ICloudinaryService
    {
        void UploadImage(IFormFile file);
    }
}
