using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Cart
{
    public interface ICartRepository
    {
        public CartItemVM GetCartItemVM(int productOptionId);
    }
}
