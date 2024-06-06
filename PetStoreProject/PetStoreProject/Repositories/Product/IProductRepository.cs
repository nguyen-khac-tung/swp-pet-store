using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Product
{
    public interface IProductRepository
    {
        public ProductDetailViewModel GetDetail(int productId);
        public List<RelatedProductViewModel> getRelatedProduct(int productId);
        public List<ProductDetailViewModel> GetProductDetailDoGet(List<int> cateId, int productCateId);
        public List<ProductDetailViewModel> GetProductDetailDoPost(List<int> cateId, int productCateId);
        public List<Brand> GetBrandsByCategoryIdsAndProductCateId(List<int> cateId, int productCateId);
        public List<Size> GetSizesByCategoryIdsAndProductCateId(List<int> cateId, int productCateId);
        public List<int> GetProductIDInWishList(int customerID);
        void AddToFavorites(int userId, int productId);
        void RemoveFromFavorites(int userId, int productId);

        public SearchResultViewModel GetListProductsByKeyWords(string key, int page);

        public int GetNumberOfDogFoods();
        public int GetNumberOfDogAccessories();
        public int GetNumberOfCatFoods();
        public int GetNumberOfCatAccessories();
        public List<HomeProductViewModel> GetProductsOfHome(int cateId, int? productCateId);
        public HomeProductViewModel GetImageAndPriceOfHomeProduct(HomeProductViewModel product);

        public List<ProductViewForAdmin> productViewForAdmins(int pageNumber, int pageSize);
        public Task<int> GetTotalProducts();
    }
}
