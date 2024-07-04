namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class PromotionViewModel
    {
        public string Name { get; set; }
        public int? Value { get; set; }
        public decimal? MaxValue { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Brand { get; set; }
        public string ProductCategory { get; set; }
        public string CreatedAt { get; set; }
    }
}
