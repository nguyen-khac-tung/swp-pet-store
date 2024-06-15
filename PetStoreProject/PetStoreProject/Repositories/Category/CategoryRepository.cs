using Microsoft.EntityFrameworkCore;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Image;
using PetStoreProject.Repositories.ProductOption;

namespace PetStoreProject.Repositories.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PetStoreDBContext _context;
        private readonly IImageRepository _image;

        

        public CategoryRepository(PetStoreDBContext context, ICategoryRepository categoryRepository, IImageRepository image)
        {
            _context = context;
            _image = image;
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
    }
}
