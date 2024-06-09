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
