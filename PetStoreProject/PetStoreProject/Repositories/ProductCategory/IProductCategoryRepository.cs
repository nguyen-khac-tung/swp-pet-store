using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.ProductCategory
{
    public interface IProductCategoryRepository
    {
        public List<ProductCategoryViewModel> GetProductCategories(int? categoryId);
        public List<ProductCategoryViewForAdmin> GetListProductCategory();
        public int CreateBrand(string BrandName);
    }
}
