using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.ProductCategory
{
    public interface IProductCategoryRepository
    {
        public List<ProductCategoryViewModel> GetProductCategories(int? categoryId);
    }
}
