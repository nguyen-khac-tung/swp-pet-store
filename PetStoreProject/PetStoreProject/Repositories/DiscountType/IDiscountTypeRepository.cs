using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.DiscountType
{
    public interface IDiscountTypeRepository
    {
        public List<DiscountTypeViewModel> GetDiscountTypes();
    }
}
