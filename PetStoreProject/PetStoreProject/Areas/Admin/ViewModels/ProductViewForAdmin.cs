namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductViewForAdmin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public float Price { get; set; }
        public bool IsSoldOut { get; set; }
        public int SoldQuantity { get; set; }
        public bool? IsDelete { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductCateId { get; set; }
    }
}
