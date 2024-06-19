using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Brand
{
    public interface IBrandRepository
    {
        public List<BrandViewModel> GetBrands();
<<<<<<< Updated upstream
        public Task<List<BrandViewForAdmin>> GetListBrand();
        public int CreateBrand(string BrandName);
=======
        public List<BrandViewForAdmin> GetListBrand();
        public int CreateBrand(string BrandName, int BrandId);
>>>>>>> Stashed changes
    }
}
