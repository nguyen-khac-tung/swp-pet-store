using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Repositories.Category;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.ProductCategory;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _product;
        private readonly ICategoryRepository _category;
        private readonly IProductCategoryRepository _productCategory;

        public ProductController(IProductRepository product, ICategoryRepository category,
            IProductCategoryRepository productCategory)
        {

            _product = product;
            _category = category;
            _productCategory = productCategory;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> fetchProduct(int? categoryId, int? productCateId, string? key,
            string? sortPrice, string? sortSoldQuantity, bool? isInStock, bool? isDelete,
            int pageNumber = 1, int pageSize = 10)
        {
            var categories = _category.GetCategories();

            categoryId = categoryId == 0 ? null : categoryId;

            var productCategories = _productCategory.GetProductCategories(categoryId);

            pageSize = Math.Min(pageSize, 30);

            ListProductForAdmin listProductForAdmin = await _product.productViewForAdmins(pageNumber, pageSize, categoryId, productCateId,
                                                                                key, sortPrice, sortSoldQuantity, isInStock, isDelete);

            var products = listProductForAdmin.products;
            var totalProduct = listProductForAdmin.totalProducts;

            var totalPageNumber = totalProduct / pageSize + 1;

            return Json(new
            {
                products = products,
                pageNumber = pageNumber,
                pageSize = pageSize,
                totalPageNumber = totalPageNumber,
                categories = categories,
                productCategories = productCategories
            });
        }

        [HttpPost]
        public JsonResult searchProductByName(string key)
        {

            return Json(new { });
        }
    }
}
