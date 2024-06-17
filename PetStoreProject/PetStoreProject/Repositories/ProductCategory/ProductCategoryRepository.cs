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

        public int CreateBrand(string BrandName)
        {
            throw new NotImplementedException();
        }

        public List<ProductCategoryViewModel> GetListProductCategory(int ProductCateId)
        {
            var totalProductsCategories = from pc in _context.ProductCategories
                                          join c in _context.Categories on pc.CategoryId equals c.CategoryId
                                          join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                                          where pc.ProductCateId == ProductCateId
                                          group pc by new { pc.ProductCateId, pc.Name } into g
                                          select new ProductCategoryViewModel
                                          {
                                              Id = g.Key.ProductCateId,
                                              Name = g.Key.Name,
                                              totalProductCategories = g.Count()
                                          };
            return totalProductsCategories.ToList();
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

        List<ProductCategoryViewForAdmin> IProductCategoryRepository.GetListProductCategory()
        {
            throw new NotImplementedException();
        }
    }
}
