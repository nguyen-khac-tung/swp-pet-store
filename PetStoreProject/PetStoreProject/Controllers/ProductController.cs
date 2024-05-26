using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PetStoreProject.Helper;
using PetStoreProject.Repositories.Product;
using PetStoreProject.ViewModels;
using System.Drawing;
using PetStoreProject.Models;

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
            var productDetails = _product.GetProductDetailAccessoriesRequest();
            var totalItems = productDetails.Count();
            var pageIndex = page ?? 1;
            var _pageSize = pageSize ?? 20;
            var numberPage = Math.Ceiling((float)totalItems / _pageSize);

            ViewBag.Brands = _product.GetBrandAccessories();
            ViewBag.totalItems = totalItems;
            ViewBag.currentPage = pageIndex;
            ViewBag.pageSize = _pageSize;
            ViewBag.numberPage = numberPage;

            var priceMax = productDetails.SelectMany(p => p.productOption).Max(po => po.price);
            var priceMin = productDetails.SelectMany(p => p.productOption).Min(po => po.price);
            ViewBag.priceMin = priceMin;
            ViewBag.priceMax = priceMax;
            return View(PaginatedList<ProductDetailViewModel>.Create(productDetails, pageIndex, _pageSize));
        }

        [HttpGet]
        public ActionResult ShopFood(int? pageSize, int? page)
        {
            var productDetails = _product.GetProductDetailFoodsRequest();
            var totalItems = productDetails.Count();
            var pageIndex = page ?? 1;
            var _pageSize = pageSize ?? 20;
            var numberPage = Math.Ceiling((float)totalItems / _pageSize);

            ViewBag.Brands = _product.GetBrandFoods();
            ViewBag.totalItems = totalItems;
            ViewBag.currentPage = pageIndex;
            ViewBag.pageSize = _pageSize;
            ViewBag.numberPage = numberPage;

            var priceMax = productDetails.SelectMany(p => p.productOption).Max(po => po.price);
            var priceMin = productDetails.SelectMany(p => p.productOption).Min(po => po.price);
            ViewBag.priceMin = priceMin;
            ViewBag.priceMax = priceMax;
            return View(PaginatedList<ProductDetailViewModel>.Create(productDetails, pageIndex, _pageSize));
        }

        [HttpPost]
        public ActionResult ShopFood(int? pageSize, int? page, List<string>? selectedBrands, string? selectedSort, int priceMin, int priceMax, List<string>? selectedColors, List<string>? selectedSizes)
        {

            List<ProductDetailViewModel> productDetails = null;
            //Filter brand
            if (!selectedBrands.IsNullOrEmpty())
                productDetails = _product.GetProductDetailFoodsResponse()
                    .Where(p => selectedBrands
                    .Contains(p.Brand) && (p.productOption[0].price >= priceMin && p.productOption[0].price <= priceMax))
                    .ToList();
            else
                productDetails = _product.GetProductDetailAccessoriesResponse()
                    .Where(p => p.productOption[0].price >= priceMin && p.productOption[0].price <= priceMax)
                    .ToList();

            if (productDetails != null)
            {
                // Filter size
                if (!selectedSizes.IsNullOrEmpty())
                    productDetails = productDetails
                        .Where(p => p.productOption
                    .Any(po => po != null && selectedSizes
                    .Any(size => po.size.Name != null && po.size.Name
                    .Contains(size))))
                        .ToList();

                // Filter color
                if (!selectedColors.IsNullOrEmpty())
                    productDetails = productDetails
                        .Where(p => p.productOption
                        .Any(po => po != null && selectedColors
                        .Any(color => po.attribute.Name != null && po.attribute.Name
                        .Contains(color))))
                        .ToList();
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



        [HttpPost]
        public ActionResult ShopAccessory(int? pageSize, int? page, List<string>? selectedBrands, string? selectedSort, int priceMin, int priceMax, List<string>? selectedColors, List<string>? selectedSizes)
        {

            List<ProductDetailViewModel> productDetails = null;
            //Filter brand
            if (!selectedBrands.IsNullOrEmpty())
                productDetails = _product.GetProductDetailAccessoriesResponse()
                    .Where(p => selectedBrands
                    .Contains(p.Brand) && (p.productOption[0].price >= priceMin && p.productOption[0].price <= priceMax))
                    .ToList();
            else
                productDetails = _product.GetProductDetailAccessoriesResponse()
                    .Where(p => p.productOption[0].price >= priceMin && p.productOption[0].price <= priceMax)
                    .ToList();

            if (productDetails != null)
            {
                // Filter size
                if (!selectedSizes.IsNullOrEmpty())
                    productDetails = productDetails
                        .Where(p => p.productOption
                    .Any(po => po != null && selectedSizes
                    .Any(size => po.size.Name != null && po.size.Name
                    .Contains(size))))
                        .ToList();

                // Filter color
                if (!selectedColors.IsNullOrEmpty())
                    productDetails = productDetails
                        .Where(p => p.productOption
                        .Any(po => po != null && selectedColors
                        .Any(color => po.attribute.Name != null && po.attribute.Name
                        .Contains(color))))
                        .ToList();
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
