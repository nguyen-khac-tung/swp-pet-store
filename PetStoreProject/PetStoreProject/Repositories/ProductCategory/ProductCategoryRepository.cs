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

        public int CreateProductCategory(string ProductCategoryName)
        {
            var productCategory = new Models.ProductCategory
            {
                Name = ProductCategoryName,
            };
            _context.ProductCategories.Add(productCategory);
            _context.SaveChanges();
            return productCategory.ProductCateId;
        }

        public List<ProductCategoryViewForAdmin> GetListProductCategory(int ProductCateId)
        {
            var ProductsCategories = (from pc in _context.ProductCategories
                                      join c in _context.Categories on pc.CategoryId equals c.CategoryId
                                      select new ProductCategoryViewForAdmin()
                                      {
                                          Id = pc.ProductCateId,
                                          ProductCateName = pc.Name,
                                          CategoryName = c.Name,
                                          IsDelete = pc.IsDelete
                                      }).ToList();

            foreach (var item in ProductsCategories)
            {
                var totalProducts = _context.Products.Where(p => p.ProductCateId == item.Id).Count();

                item.TotalProducts = totalProducts;

                var totalQuantitySold = (from pc in ProductsCategories
                                         join p in _context.Products on pc.Id equals p.ProductCateId
                                         join po in _context.ProductOptions on p.ProductId equals po.ProductId
                                         join od in _context.OrderItems on po.ProductOptionId equals od.ProductOptionId
                                         where pc.Id == item.Id
                                         select od.Quantity).Sum();

                item.QuantitySoldProduct = totalQuantitySold;
            }

            return ProductsCategories;
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
