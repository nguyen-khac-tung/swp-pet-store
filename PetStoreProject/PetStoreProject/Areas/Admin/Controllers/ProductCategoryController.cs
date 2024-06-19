using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.Category;
using PetStoreProject.Repositories.ProductCategory;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductCategoryController : Controller
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IBrandRepository _brand;
        private readonly ICategoryRepository _category;

        public ProductCategoryController(IProductCategoryRepository productCategoryRepository,
            IBrandRepository brand, ICategoryRepository category)
        {
            _productCategoryRepository = productCategoryRepository;
            _brand = brand;
            _category = category;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var productCategories = _productCategoryRepository.GetListProductCategory();
            ViewData["ProductCategories"] = productCategories;
            var brands = await _brand.GetListBrand();
            ViewData["brands"] = brands;
            var categories = _category.GetCategories();
            ViewData["categories"] = categories;
            return View();
        }


    }
}
