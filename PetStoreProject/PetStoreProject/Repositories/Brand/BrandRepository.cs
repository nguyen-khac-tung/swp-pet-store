using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Brand
{
    public class BrandRepository : IBrandRepository
    {
        private readonly PetStoreDBContext _context;

        public BrandRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public int CreateBrand(string BrandName)
        {
            var brandId = _context.Brands.Max(b => b.BrandId) + 1;
            var brand = new Models.Brand
            {
                BrandId = brandId,
                Name = BrandName,
            };

            _context.Brands.Add(brand);
            _context.SaveChanges();
            return brandId;
        }

        public List<BrandViewModel> GetBrands()
        {
            var brands = _context.Brands.ToList();

            List<BrandViewModel> brandViewModels = new List<BrandViewModel>();
            foreach (var brand in brands)
            {
                brandViewModels.Add(new BrandViewModel
                {
                    Id = brand.BrandId,
                    Name = brand.Name,
                    totalBrands = GetListBrand(brand.BrandId).Count()
                });
            }
            return brandViewModels;
        }



        public List<BrandViewModel> GetListBrand(int BrandId)
        {
            var totalProductsByBrand = from b in _context.Brands
                                       join p in _context.Products on b.BrandId equals p.BrandId
                                       where b.BrandId == BrandId 
                                       group b by new { b.BrandId, b.Name } into g
                                       select new BrandViewModel 
                                       {
                                           Id = g.Key.BrandId, 
                                           Name = g.Key.Name, 
                                           totalBrands = g.Count() 
                                       };
            return totalProductsByBrand.ToList(); 
        }

        List<BrandViewForAdmin> IBrandRepository.GetListBrand()
        {
            var brands = _context.Brands.Select(c => new BrandViewForAdmin
            {
                Id = c.BrandId,
                Name = c.Name
            }).ToList();
            foreach (var brand in brands)
            {
                brand.Quantity = (from b in brands
                                  join p in _context.Products on b.Id equals p.BrandId
                                  where b.Id == brand.Id
                                  select b).Count();
                brand.QuantityOfSold = (from b in brands
                                        join p in _context.Products on b.Id equals p.BrandId
                                        join pc in _context.ProductCategories on p.ProductCateId equals pc.ProductCateId
                                        join po in _context.ProductOptions on p.ProductId equals po.ProductId
                                        join od in _context.OrderItems on po.ProductOptionId equals od.ProductOptionId
                                        select po).Count();
            }
            return brands;
        }
    }
}
