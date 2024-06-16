using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Brand
{
    public interface IBrandRepository
    {
        public List<BrandViewModel> GetBrands();
        public List<BrandViewForAdmin> GetListBrand();
        public int CreateBrand(string BrandName);
    }
}
