namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductDetailForAdmin
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public bool? IsSoldOut { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
