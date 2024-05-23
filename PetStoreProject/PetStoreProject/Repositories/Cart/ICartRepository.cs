using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Cart
{
    public interface ICartRepository
    {
        public CartItemViewModel GetCartItemVM(int productOptionId);
    }
}
