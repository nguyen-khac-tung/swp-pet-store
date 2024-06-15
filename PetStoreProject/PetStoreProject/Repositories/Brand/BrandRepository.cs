using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Brand
{
    public class BrandRepository : IBrandRepository
    {
        private readonly PetStoreDBContext _context;

        public BrandRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public int CreateBrand(string brandName)
        {
            var brand = new Models.Brand
            {
                Name = brandName
            };
            _context.Brands.Add(brand);
            _context.SaveChanges();
            return brand.BrandId;
        }

        public List<BrandViewModel> GetBrands()
        {
            var brands = _context.Brands.Select(b => new BrandViewModel
            {
                Id = b.BrandId,
                Name = b.Name
            }).ToList();
            return brands;
        }
    }
}
