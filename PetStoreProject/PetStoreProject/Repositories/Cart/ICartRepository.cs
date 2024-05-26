using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Cart
{
	public interface ICartRepository
	{
		public CartItemViewModel GetCartItemVM(int productOptionId, int quantity);
		public List<CartItemViewModel> GetListCartItemsVM(int customerId);

		public void AddItemsToCart(int productOptionId, int quantity, int customerID);

		public void UpdateQuantityToCartItem(int productOptionId, int quantity, int customerID);

		public void DeleteCartItem(int productOptionId, int customer);
    }
}
