using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Promotion
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly PetStoreDBContext _context;

        public PromotionRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public void CreatePromotion(PromotionCreateRequest promotion)
        {
            var id = 1;

            try
            {
                id = _context.Promotions.Max(x => x.PromotionId) + 1;
            }
            catch
            {
                id = 1;
            }

            var p = new Models.Promotion()
            {
                PromotionId = id,
                Name = promotion.Name,
                Value = promotion.Value,
                MaxValue = promotion.MaxValue,
                StartDate = promotion.StartDate.ToString(),
                EndDate = promotion.EndDate.ToString(),
                BrandId = promotion.BrandId,
                ProductCateId = promotion.ProductCateId,
                CreatedAt = DateTime.Now.ToString(),
            };
            _context.Promotions.Add(p);
            _context.SaveChanges();
        }

        public List<Models.Promotion> GetPromotions()
        {
            var promotion = _context.Promotions.ToList();
            return promotion;
        }
    }
}
