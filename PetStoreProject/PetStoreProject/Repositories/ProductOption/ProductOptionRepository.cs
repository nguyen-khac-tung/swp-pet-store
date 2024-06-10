using Microsoft.EntityFrameworkCore;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Image;

namespace PetStoreProject.Repositories.ProductOption
{
    public class ProductOptionRepository : IProductOptionRepository
    {
        private readonly PetStoreDBContext _context;
        private readonly IImageRepository _image;

        public ProductOptionRepository(PetStoreDBContext context, IImageRepository image)
        {
            _context = context;
            _image = image;
        }

        public async Task<string> CreateProductOption(ProductOptionCreateRequestViewModel productOptionCreateRequest, int productId)
        {
            try
            {
                var productOptionId = await _context.ProductOptions.MaxAsync(p => p.ProductOptionId) + 1;

                var imageId = await _image.CreateImage(productOptionCreateRequest.ImageData);

                if (!int.TryParse(imageId, out int number))
                {
                    return imageId;
                }

                var productOption = new PetStoreProject.Models.ProductOption
                {
                    ProductId = productId,
                    ImageId = int.Parse(imageId),
                    Price = productOptionCreateRequest.Price,
                    SizeId = productOptionCreateRequest.SizeId,
                    AttributeId = productOptionCreateRequest.AttributeId
                };

                await _context.ProductOptions.AddAsync(productOption);
                await _context.SaveChangesAsync();

                return productOptionId.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
