using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Discount
{
    public interface IDiscountRepository
    {
        public string Create(Models.Discount discount);
        public List<DiscountViewModel> GetDiscounts();
        public DiscountViewModel GetDiscount(int id);
        public string Edit(Models.Discount discount);
        public List<DiscountViewModel> GetDiscounts(double total_amount, string email);
    }
}
