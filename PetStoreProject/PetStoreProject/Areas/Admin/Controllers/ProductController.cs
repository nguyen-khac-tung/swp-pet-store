using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Repositories.Product;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _product;

        public ProductController(IProductRepository product)
        {
            _product = product;
        }

        [HttpGet]
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 10)
        {
            var totalProduct = await _product.GetTotalProducts();
            pageSize = Math.Min(pageSize, 30);
            var totalPageNumber = totalProduct / pageSize + 1;
            pageNumber = Math.Min(pageNumber, totalPageNumber);
            List<ProductViewForAdmin> products = _product.productViewForAdmins(pageNumber, pageSize);
            ViewData["products"] = products;
            ViewData["pageNumber"] = pageNumber;
            ViewData["pageSize"] = pageSize;
            ViewData["totalPageNumber"] = totalPageNumber;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> fetchProduct(int pageNumber = 1, int pageSize = 10)
        {
            var totalProduct = await _product.GetTotalProducts();
            pageSize = Math.Min(pageSize, 30);
            var totalPageNumber = totalProduct / pageSize + 1;
            pageNumber = Math.Min(pageNumber, totalPageNumber);
            List<ProductViewForAdmin> products = _product.productViewForAdmins(pageNumber, pageSize);
            return Json(new
            {
                products = products,
                pageNumber = pageNumber,
                pageSize = pageSize,
                totalPageNumber = totalPageNumber
            });
        }
    }
}
