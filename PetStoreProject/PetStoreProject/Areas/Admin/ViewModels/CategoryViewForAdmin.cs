namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class CategoryViewForAdmin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public bool? IsDelete { get; set; }
    }
}
