using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Category
{
    public interface ICategoryRepository
    {
        public List<CategoryViewModel> GetCategories();
        public List<CategoryViewModel> GetListCategory(int CategoryId);
    }

}
