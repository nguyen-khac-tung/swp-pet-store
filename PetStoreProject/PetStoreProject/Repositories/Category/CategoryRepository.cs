using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PetStoreDBContext _context;

        public CategoryRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public int CreateCategory(string CategoryName)
        {
            var cateId = _context.Categories.Max(c => c.CategoryId) + 1;
            var category = new Models.Category
            {
                CategoryId = cateId,
                Name = CategoryName,
            };

            _context.Categories.Add(category);
            _context.SaveChanges();
            return cateId;
        }

        public List<CategoryViewModel> GetCategories()
        {
            var categories = _context.Categories.ToList();

            List<CategoryViewModel> categoryViewModels = new List<CategoryViewModel>();
            foreach (var category in categories)
            {
                categoryViewModels.Add(new CategoryViewModel
                {
                    Id = category.CategoryId,
                    Name = category.Name,
                    totalCategories = GetListCategory(category.CategoryId).Count()
                });
            }
            return categoryViewModels;
        }
        public List<CategoryViewModel> GetListCategory(int CategoryId)
        {
            var totalProductsByCategory = from c in _context.Categories
                                          join pc in _context.ProductCategories on c.CategoryId equals pc.CategoryId
                                          join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                                          where c.CategoryId == CategoryId // Filtering by CategoryId
                                          group p by new { c.CategoryId, c.Name } into g
                                          select new CategoryViewModel // Projecting into CategoryViewModel
                                          {
                                              Id = g.Key.CategoryId, // Assuming CategoryViewModel has an Id property
                                              Name = g.Key.Name, // Using Name for the category name
                                              totalCategories = g.Count() // Assuming you add a TotalProducts property to CategoryViewModel
                                          };

            return totalProductsByCategory.ToList(); // Converting the result to List<CategoryViewModel>
        }

        List<CategoryViewForAdmin> ICategoryRepository.GetListCategory()
        {
            var categories = _context.Categories.Select(c => new CategoryViewForAdmin
            {
                Id = c.CategoryId,
                Name = c.Name
            }).ToList();

            foreach (var category in categories)
            {
                category.Quantity = (from c in categories
                                     join pc in _context.ProductCategories on c.Id equals pc.CategoryId
                                     join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                                     where c.Id == category.Id
                                     select p).Count();
                category.QuantityOfSold = (from c in categories
                                           join pc in _context.ProductCategories on c.Id equals pc.CategoryId
                                           join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                                           join po in _context.ProductOptions on p.ProductId equals po.ProductId
                                           join od in _context.OrderItems on po.ProductOptionId equals od.ProductOptionId
                                           select po).Count();
            }

            return categories;
        }
    }
}
