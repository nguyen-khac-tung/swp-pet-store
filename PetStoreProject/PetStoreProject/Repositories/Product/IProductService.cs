using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Product
{
	public interface IProductService
	{
		public ProductDetailVM GetDetail(int productId);
		public List<RelatedProductVM> getRelatedProduct(int productId);
	}
}
