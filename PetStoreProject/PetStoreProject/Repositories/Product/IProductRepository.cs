using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Product
{
	public interface IProductRepository
	{
		public ProductDetailViewModel GetDetail(int productId);
		public List<RelatedProductViewModel> getRelatedProduct(int productId);

		public List<ProductDetailViewModel> GetProductDetailAccessaries();

    }
}
