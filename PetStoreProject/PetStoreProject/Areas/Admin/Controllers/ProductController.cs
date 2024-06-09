using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Repositories.Attribute;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.Category;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.ProductCategory;
using PetStoreProject.Repositories.Size;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _product;
        private readonly ICategoryRepository _category;
        private readonly IProductCategoryRepository _productCategory;
        private readonly ICloudinaryService _cloudinary;
        private readonly IAttributeRepository _attribute;
        private readonly ISizeRepository _size;
        private readonly IBrandRepository _brand;

        public ProductController(IProductRepository product, ICategoryRepository category,
            IProductCategoryRepository productCategory, ICloudinaryService cloudinary,
            IBrandRepository brand, IAttributeRepository attribute, ISizeRepository size)
        {
            _product = product;
            _category = category;
            _productCategory = productCategory;
            _cloudinary = cloudinary;
            _brand = brand;
            _attribute = attribute;
            _size = size;
        }

        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> fetchProduct(int? categoryId, int? productCateId, string? key,
            bool? sortPrice, bool? sortSoldQuantity, bool? isInStock, bool? isDelete,
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
                productCategories = productCategories,
                key = key
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _category.GetCategories();
            var productCategories = _productCategory.GetProductCategories(null);
            var attributes = _attribute.GetAttributes();
            var sizes = _size.GetSizes();
            var brands = _brand.GetBrands();
            ViewData["categories"] = categories;
            ViewData["productCategories"] = productCategories;
            ViewData["attributes"] = attributes;
            ViewData["sizes"] = sizes;
            ViewData["brands"] = brands;
            return View();
        }
    }
}
