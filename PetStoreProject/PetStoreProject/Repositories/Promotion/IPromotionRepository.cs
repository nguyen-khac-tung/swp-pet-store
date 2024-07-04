using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Promotion
{
    public interface IPromotionRepository
    {
        public void CreatePromotion(PromotionCreateRequest promotion);
        public List<Models.Promotion> GetPromotions();
    }
}
