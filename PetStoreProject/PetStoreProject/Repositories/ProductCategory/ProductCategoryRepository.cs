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
                id = pc.ProductCateId,
                name = pc.Name.Trim(),
                categoryId = pc.CategoryId
            }).ToList();

            if (categoryId != null)
            {
                productCategories = productCategories.Where(pc => pc.categoryId == categoryId).ToList();
            }
            return productCategories;

        }
    }
}
