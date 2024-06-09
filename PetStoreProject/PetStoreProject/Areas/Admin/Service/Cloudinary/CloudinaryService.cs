using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace PetStoreProject.Areas.Admin.Service.Cloudinary
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryService(CloudinaryDotNet.Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public void UploadImage(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = "sample_image",
                    Overwrite = true
                };

                var uploadResult = _cloudinary.Upload(uploadParams);
                if (uploadResult.Error != null)
                {
                    Console.WriteLine($"Lỗi khi tải lên ảnh: {uploadResult.Error.Message}");
                }
                else
                {
                    Console.WriteLine(uploadResult.Url);
                }
            }
        }
    }
}
