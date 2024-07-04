using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Promotion
{
    public interface IPromotionRepository
    {
        public void CreatePromotion(PromotionCreateRequest promotion);
        public List<PromotionViewModel> GetPromotions();
    }
}
