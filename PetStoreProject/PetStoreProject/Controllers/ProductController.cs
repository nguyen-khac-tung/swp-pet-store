using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PetStoreProject.Helper;
using PetStoreProject.Repositories.Product;
using PetStoreProject.ViewModels;
using System.Drawing;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Customers;

namespace PetStoreProject.Controllers
{
    [Route("[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _product;
        private readonly ICustomerRepository _customer;

        public ProductController(IProductRepository product, ICustomerRepository customer)
        {
            _product = product;
            _customer = customer;
        }

        public int getCustomerId()
        {
            var email = HttpContext.Session.GetString("Account");
            if (email != null)
            {
                var customerID = _customer.getCustomerId(email);
                return customerID;
            }
            else
            {
                return -1;
            }
        }

        [HttpPost]
        public IActionResult ToggleFavorite(int productId)
        {

            var favoriteList = _product.GetProductIDInWishList(getCustomerId());

            if (favoriteList.Contains(productId))
            {
                _product.RemoveFromFavorites(getCustomerId(), productId);
            }
            else
            {
                _product.AddToFavorites(getCustomerId(), productId);
            }
            return Json(new { success = true });
        }

        [HttpGet("{productId}")]
        public ActionResult Detail(int productId)
        {
            var product_detail = _product.GetDetail(productId);
			List<int> listPID = _product.GetProductIDInWishList(getCustomerId());

			ViewData["product_detail"] = product_detail;
            ViewData["related_products"] = _product.getRelatedProduct(productId);
			ViewData["listPID"] = listPID;

			return View();
        }

        [HttpPost]
        public ActionResult quickPreview(int productId)
        {
            var product_detail = _product.GetDetail(productId);
            return Json(product_detail);
        }

        [HttpGet("/Product/DogFood/{productCateId?}")]
        public ActionResult DogFood(int? productCateId, int? pageSize, int? page)
        {
            List<int> cateId = [1, 3];
            var productDetails = _product.GetProductDetailFoodsRequest(cateId, productCateId ?? 0); // thay doi
            var totalItems = productDetails.Count();
            var pageIndex = page ?? 1;
            var _pageSize = pageSize ?? 20;
            var numberPage = Math.Ceiling((float)totalItems / _pageSize);
            List<int> listPID = _product.GetProductIDInWishList(getCustomerId());

            ViewData["listPID"] = listPID;
            ViewBag.Brands = _product.GetBrandsByCategoryIdsAndProductCateId(cateId, productCateId ?? 0);
            ViewBag.totalItems = totalItems;
            ViewBag.currentPage = pageIndex;
            ViewBag.pageSize = _pageSize;
            ViewBag.numberPage = numberPage;



            if (productDetails.Count != 0)
            {
                var priceMax = productDetails.SelectMany(p => p.productOption).Max(po => po.price);
                var priceMin = productDetails.SelectMany(p => p.productOption).Min(po => po.price);
                ViewBag.priceMin = priceMin;
                ViewBag.priceMax = priceMax;
            }

            return View("Food", PaginatedList<ProductDetailViewModel>.Create(productDetails, pageIndex, _pageSize));
        }
        [HttpGet("/Product/DogAccessory/{productCateId?}")]
        public ActionResult DogAccessory(int? productCateId, int? pageSize, int? page)
        {
            List<int> cateId = [2,5];
            var productDetails = _product.GetProductDetailAccessoriesRequest(cateId, productCateId ?? 0); // thay doi
            var totalItems = productDetails.Count();
            var pageIndex = page ?? 1;
            var _pageSize = pageSize ?? 20;
            var numberPage = Math.Ceiling((float)totalItems / _pageSize);
            List<int> listPID = _product.GetProductIDInWishList(getCustomerId());

            ViewData["listPID"] = listPID;
            ViewBag.Brands = _product.GetBrandsByCategoryIdsAndProductCateId(cateId, productCateId ?? 0);
            ViewBag.totalItems = totalItems;
            ViewBag.currentPage = pageIndex;
            ViewBag.pageSize = _pageSize;
            ViewBag.numberPage = numberPage;


            if(productDetails.Count != 0)
            {
                var priceMax = productDetails.SelectMany(p => p.productOption).Max(po => po.price);
                var priceMin = productDetails.SelectMany(p => p.productOption).Min(po => po.price);
                ViewBag.priceMin = priceMin;
                ViewBag.priceMax = priceMax;
            }
            
            return View("Accessory", PaginatedList<ProductDetailViewModel>.Create(productDetails, pageIndex, _pageSize));
        }

        [HttpGet("/Product/CatFood/{productCateId?}")]
        public ActionResult CatFood(int? productCateId, int? pageSize, int? page)
        {
            List<int> cateId = [1, 4];
            var productDetails = _product.GetProductDetailAccessoriesRequest(cateId, productCateId ?? 0); // thay doi
            var totalItems = productDetails.Count();
            var pageIndex = page ?? 1;
            var _pageSize = pageSize ?? 20;
            var numberPage = Math.Ceiling((float)totalItems / _pageSize);
            List<int> listPID = _product.GetProductIDInWishList(getCustomerId());

            ViewData["listPID"] = listPID;
            ViewBag.Brands = _product.GetBrandsByCategoryIdsAndProductCateId(cateId, productCateId ?? 0);
            ViewBag.totalItems = totalItems;
            ViewBag.currentPage = pageIndex;
            ViewBag.pageSize = _pageSize;
            ViewBag.numberPage = numberPage;



            if (productDetails.Count != 0)
            {
                var priceMax = productDetails.SelectMany(p => p.productOption).Max(po => po.price);
                var priceMin = productDetails.SelectMany(p => p.productOption).Min(po => po.price);
                ViewBag.priceMin = priceMin;
                ViewBag.priceMax = priceMax;
            }

            return View("Food", PaginatedList<ProductDetailViewModel>.Create(productDetails, pageIndex, _pageSize));
        }
        [HttpGet("/Product/CatAccessory/{productCateId?}")]
        public ActionResult CatAccessory(int? productCateId ,int? pageSize, int? page)
        {
            List<int> cateId = [2, 6];
            var productDetails = _product.GetProductDetailAccessoriesRequest(cateId, productCateId ?? 0); // thay doi
            var totalItems = productDetails.Count();
            var pageIndex = page ?? 1;
            var _pageSize = pageSize ?? 20;
            var numberPage = Math.Ceiling((float)totalItems / _pageSize);
            List<int> listPID = _product.GetProductIDInWishList(getCustomerId());

            ViewData["listPID"] = listPID;
            ViewBag.Brands = _product.GetBrandsByCategoryIdsAndProductCateId(cateId, productCateId ?? 0);
            ViewBag.totalItems = totalItems;
            ViewBag.currentPage = pageIndex;
            ViewBag.pageSize = _pageSize;
            ViewBag.numberPage = numberPage;

            if (productDetails.Count != 0)
            {
                var priceMax = productDetails.SelectMany(p => p.productOption).Max(po => po.price);
                var priceMin = productDetails.SelectMany(p => p.productOption).Min(po => po.price);
                ViewBag.priceMin = priceMin;
                ViewBag.priceMax = priceMax;
            }

            return View("Accessory", PaginatedList<ProductDetailViewModel>.Create(productDetails, pageIndex, _pageSize));
        }

        [HttpPost]
        public ActionResult Shop(string url, int? pageSize, int? page, List<string>? selectedBrands, string? selectedSort, int priceMin,
            int priceMax, List<string>? selectedColors, List<string>? selectedSizes)
        {
            List<int> cateId = null;
            int productCateId = 0;
            var urlSplit = url.Split("/").ToList();

            switch (urlSplit[2])
            {
                case "DogFood":
                    cateId = new List<int> { 1, 3 };
                    break;
                case "CatFood":
                    cateId = new List<int> { 1, 4 };
                    break;
                case "DogAccessory":
                    cateId = new List<int> { 2, 5 };
                    break;
                case "CatAccessory":
                    cateId = new List<int> { 2, 6 };
                    break;
            }
            if (urlSplit.Count > 3 && urlSplit[3] != null)
                {
                    productCateId = int.Parse(urlSplit[3]);
                }
            List<ProductDetailViewModel> productDetails = null;

            //Filter brand
            if (!selectedBrands.IsNullOrEmpty())
                productDetails = _product.GetProductDetailFoodsResponse(cateId, productCateId)
                    .Where(p => selectedBrands
                    .Contains(p.Brand) && (p.productOption[0].price >= priceMin && p.productOption[0].price <= priceMax))
                    .ToList();
            else
                productDetails = _product.GetProductDetailFoodsResponse(cateId, productCateId)
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
