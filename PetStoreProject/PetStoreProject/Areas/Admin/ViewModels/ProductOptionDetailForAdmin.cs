namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductOptionDetailForAdmin
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public int SizeId { get; set; }
        public float Price { get; set; }
        public string? ImgUrl { get; set; }
        public bool? IsSoldOut { get; set; }
        public bool? IsDelete { get; set; }
    }
}
