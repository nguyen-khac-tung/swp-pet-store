using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Product
{
	public interface IProductRepository
	{
		public ProductDetailVM GetDetail(int productId);
		public List<RelatedProductVM> getRelatedProduct(int productId);

		public List<ProductDetailVM> GetProductDetailAccessories();

		public List<Brand> GetBrandAccessories();

    }
}
