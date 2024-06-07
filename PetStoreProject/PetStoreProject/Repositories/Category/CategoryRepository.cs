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

        public List<CategoryViewModel> GetCategories()
        {
            var categories = _context.Categories.ToList();

            List<CategoryViewModel> categoryViewModels = new List<CategoryViewModel>();
            foreach (var category in categories)
            {
                categoryViewModels.Add(new CategoryViewModel
                {
                    id = category.CategoryId,
                    name = category.Name
                });
            }
            return categoryViewModels;
        }
    }
}
