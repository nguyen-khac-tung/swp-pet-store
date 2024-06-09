using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.ProductCategory
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly PetStoreDBContext _context;

        public ProductCategoryRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public List<ProductCategoryViewModel> GetProductCategories(int? categoryId)
        {
            var productCategories = _context.ProductCategories.Select(pc => new ProductCategoryViewModel
            {
                Id = pc.ProductCateId,
                Name = pc.Name.Trim(),
                CategoryId = pc.CategoryId
            }).ToList();

            if (categoryId != null)
            {
                productCategories = productCategories.Where(pc => pc.CategoryId == categoryId).ToList();
            }
            return productCategories;

        }
    }
}
