using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Product
{
	public interface IProductRepository
	{
		public ProductDetailVM GetDetail(int productId);
		public List<RelatedProductVM> getRelatedProduct(int productId);
	}
}
