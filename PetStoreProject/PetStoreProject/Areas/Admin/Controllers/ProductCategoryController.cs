using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.ProductCategory;

namespace PetStoreProject.Areas.Admin.Controllers
{
    public class ProductCategoryController : Controller
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IBrandRepository _brand;

        public ProductCategoryController(IProductCategoryRepository productCategoryRepository,
            IBrandRepository brand)
        {
            _productCategoryRepository = productCategoryRepository;
            _brand = brand;
        }

        [HttpGet]
        public IActionResult List()
        {
            var productCategories = _productCategoryRepository.GetListProductCategory();
            ViewData["ProductCategories"] = productCategories;
            var brands = _brand.GetListBrand();
            ViewData["brands"] = brands;
            return View();
        }
    }
}
