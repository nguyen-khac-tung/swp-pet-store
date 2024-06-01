using PetStoreProject.Models;

namespace PetStoreProject.ViewModels
{
    public class RelatedProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public List<Image> images { get; set; }
        public string brand { get; set; }
        public bool IsSoldOut { get; set; }
    }
}
