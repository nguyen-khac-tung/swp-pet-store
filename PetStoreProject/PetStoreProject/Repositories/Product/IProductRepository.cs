using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Product
{
	public interface IProductRepository
	{
		public ProductDetailViewModel GetDetail(int productId);
		public List<RelatedProductViewModel> getRelatedProduct(int productId);
		public List<ProductDetailViewModel> GetProductDetailAccessoriesRequest();
		public List<ProductDetailViewModel> GetProductDetailFoodsRequest();
		public List<ProductDetailViewModel> GetProductDetailAccessoriesResponse();
		public List<ProductDetailViewModel> GetProductDetailFoodsResponse();
        public List<Brand> GetBrandAccessories();
		public List<Brand> GetBrandFoods();
		public List<int> GetProductIDInWishList(int customerID);
        void AddToFavorites(int userId, int productId);
        void RemoveFromFavorites(int userId, int productId);
    }
}
