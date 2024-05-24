using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PetStoreProject.Helper;
using PetStoreProject.Repositories.Product;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Controllers
{
    [Route("[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _product;

        public ProductController(IProductRepository product)
        {
            _product = product;
        }

        [HttpGet("{productId}")]
        public ActionResult Detail(int productId)
        {
            var product_detail = _product.GetDetail(productId);
            ViewData["product_detail"] = product_detail;
            ViewData["related_products"] = _product.getRelatedProduct(productId);
            return View();
        }

        [HttpPost]
        public ActionResult quickPreview(int productId)
        {
            var product_detail = _product.GetDetail(productId);
            return Json(product_detail);
        }

        [HttpGet]
        public ActionResult ShopAccessory(int? pageSize, int? page)
        {
            var productDetails = _product.GetProductDetailAccessories();
            var totalItems = productDetails.Count();
            var pageIndex = page ?? 1;
            var _pageSize = pageSize ?? 20;
            var numberPage = Math.Ceiling((float)totalItems / _pageSize);

            ViewBag.Brands = _product.GetBrandAccessories();
            ViewBag.totalItems = totalItems;
            ViewBag.currentPage = pageIndex;
            ViewBag.pageSize = _pageSize;
            ViewBag.numberPage = numberPage;
            return View(PaginatedList<ProductDetailViewModel>.Create(productDetails, pageIndex, _pageSize));
        }

        [HttpPost]
        public ActionResult ShopAccessory_Post(int? pageSize, int? page, List<string>? selectedBrands, string? selectedSort)
        {
            List<ProductDetailViewModel> productDetails = null;
            int count = 0;
            if (selectedBrands.IsNullOrEmpty())
            {
                productDetails = _product.GetProductDetailAccessories();
            }
            else
            {
                productDetails = _product.GetProductDetailAccessories().Where(p => selectedBrands.Contains(p.Brand)).ToList();
                count = productDetails.Count();
            }
            if (!selectedSort.IsNullOrEmpty())
            {
                switch (selectedSort)
                {
                    case "ProductNameAZ":
                        productDetails = productDetails.OrderBy(p => p.Name).ToList();
                        break;
                    case "ProductNameZA":
                        productDetails = productDetails.OrderByDescending(p => p.Name).ToList();
                        break;
                    case "PriceLowToHight":
                        productDetails = productDetails.OrderBy(p => p.productOption[0].price).ToList();
                        break;
                    case "PriceHightToLow":
                        productDetails = productDetails.OrderByDescending(p => p.productOption[0].price).ToList();
                        break;
                }
            }
            var _pageSize = pageSize ?? 20;
            var pageIndex = page ?? 1;
            var totalItems = productDetails.Count();
            var numberPage = Math.Ceiling((float)totalItems / _pageSize);
            var productDetail = productDetails.Skip((pageIndex - 1) * _pageSize).Take(_pageSize).ToList();
            Console.WriteLine(count);

            TempData["pageSize"] = _pageSize;
            TempData["totalItems"] = totalItems;
            TempData["currentPage"] = pageIndex;
            return new JsonResult(new
            {
                Data = productDetail,
                CurrentPage = pageIndex,
                NumberPage = numberPage,
                PageSize = _pageSize,
            });

        }

    }
}
