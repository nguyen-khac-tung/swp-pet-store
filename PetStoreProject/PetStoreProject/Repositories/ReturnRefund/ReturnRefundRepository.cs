using CloudinaryDotNet.Actions;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.ReturnRefund
{
    public class ReturnRefundRepository : IReturnRefundRepository
    {
        private readonly PetStoreDBContext _context;
        private readonly ICloudinaryService _cloudinary;

        public ReturnRefundRepository(PetStoreDBContext context, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinary = cloudinaryService;
        }
        public List<Models.ReturnRefund> GetReturnRefunds()
        {
            return _context.ReturnRefunds.ToList();
        }

        public async Task AddImageToReturnRefund(int returnId, ViewModels.CreateReturnRefund returnRefund)
        {
            int maxImageId = (from i in _context.Images
                              orderby i.ImageId descending
                              select i.ImageId).FirstOrDefault();
            foreach (var imageData in returnRefund.images)
            {
                maxImageId++;
                ImageUploadResult result = await _cloudinary.UploadImage(imageData, "image_" + maxImageId);
                Models.Image image = new Models.Image
                {
                    ImageId = maxImageId,
                    ReturnId = returnId,
                    ImageUrl = result.Url.ToString()
                };
                _context.Images.Add(image);
            }
        }
        private bool IsBase64String(string imageData)
        {
            if (imageData.Contains("cloudinary"))
            {
                return false;
            }
            try
            {
                var base64Data = imageData.Split(',')[1];
                Convert.FromBase64String(base64Data);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public void CreateReturnRefund(ViewModels.CreateReturnRefund returnRefund)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Models.ReturnRefund returnRef = new Models.ReturnRefund
                    {
                        ReasonReturn = returnRefund.reasonReturn,
                        Status = "Yêu cầu trả hàng",
                        ResponseContent = null
                    };
                    _context.ReturnRefunds.Add(returnRef);
                    _context.SaveChanges();
                    var returnId = returnRef.ReturnId;
                    AddImageToReturnRefund(returnId, returnRefund);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }
    }
}
