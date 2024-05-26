using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Product
{
	public interface IProductRepository
	{
		public ProductDetailViewModel GetDetail(int productId);
		public List<RelatedProductViewModel> getRelatedProduct(int productId);
		public List<ProductDetailViewModel> GetProductDetailAccessoriesRequest(List<int> cateId, int productCateId);

        public List<ProductDetailViewModel> GetProductDetailFoodsRequest(List<int> cateId, int productCateId);
		public List<ProductDetailViewModel> GetProductDetailAccessoriesResponse(List<int> cateId, int productCateId);

        public List<ProductDetailViewModel> GetProductDetailFoodsResponse(List<int> cateId, int productCateId);
		public List<Brand> GetBrandsByCategoryIdsAndProductCateId(List<int> cateId, int productCateId);
		
		public List<int> GetProductIDInWishList(int customerID);


	}
}
