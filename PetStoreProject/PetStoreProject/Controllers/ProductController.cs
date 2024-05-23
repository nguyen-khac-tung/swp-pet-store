using Microsoft.AspNetCore.Mvc;
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
        public ActionResult ShopAccessary(int? pageSize, int? page)
        {
            var productDetails = _product.GetProductDetailAccessories();
            ViewBag.Brands = _product.GetBrandAccessories();
            var totalPage = productDetails.Count();
            
            ViewBag.totalPage = totalPage;
            var _page = page ?? 1;
            var _pageSize = pageSize ?? 20;
            ViewBag.currentPage = _page;
            ViewBag.pageSize = _pageSize;
            ViewBag.page = _pageSize;
            return View(PaginatedList<ProductDetailViewModel>.Create(productDetails, _page, _pageSize));
        }

        [HttpGet]
        public ActionResult Shop(int? pageSize, int? page)
        {
            var productDetails = _product.GetProductDetailAccessories();
            var _pageSize = pageSize ?? 20;
            var pageIndex = page ?? 1;
            var totalPage = productDetails.Count();
            var numberPage = Math.Ceiling((float)totalPage / _pageSize);
            var productDetail = productDetails.Skip((pageIndex - 1) * _pageSize).Take(_pageSize).ToList();

            ViewBag.pageSize = _pageSize;
            ViewBag.totalPage = totalPage;
            ViewBag.currentPage = page;

            //return Json(productDetails);
            return new JsonResult(new
            {
                Data = productDetail,
                TotaItems = totalPage,
                CurrentPage = pageIndex,
                NumberPage = numberPage,
                PageSize = _pageSize,
            });
        }

    }
}
