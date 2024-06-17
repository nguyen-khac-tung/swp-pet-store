namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductCateId { get; set; }
        public int TotalProduct { get; set; }

        public int QuanitySoldProduct { get; set; }
    }
}
