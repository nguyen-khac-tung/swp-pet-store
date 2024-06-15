using Microsoft.EntityFrameworkCore;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.ProductOption
{
    public class ProductOptionRepository : IProductOptionRepository
    {
        private readonly PetStoreDBContext _context;

        public ProductOptionRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public async Task<string> CreateProductOption(ProductOptionCreateRequestViewModel productOptionCreateRequest, int productId, int imageId)
        {
            try
            {
                var productOptionId = await _context.ProductOptions.MaxAsync(p => p.ProductOptionId) + 1;

                var productOption = new PetStoreProject.Models.ProductOption
                {
                    ProductId = productId,
                    ImageId = imageId,
                    Price = productOptionCreateRequest.Price,
                    SizeId = productOptionCreateRequest.Size.SizeId,
                    AttributeId = productOptionCreateRequest.Attribute.AttributeId
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
