using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.ProductCategory
{
    public interface IProductCategoryRepository
    {
        public List<ProductCategoryViewModel> GetProductCategories(int? categoryId, bool getDeleted);
        public List<ProductCategoryViewForAdmin> GetListProductCategory();
        public int CreateProductCategory(string ProductCategoryName, int CategoryId);
    }
}
