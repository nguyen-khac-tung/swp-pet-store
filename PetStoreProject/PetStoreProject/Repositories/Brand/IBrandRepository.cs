using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Brand
{
    public interface IBrandRepository
    {
        public List<BrandViewModel> GetBrands();
    }
}
