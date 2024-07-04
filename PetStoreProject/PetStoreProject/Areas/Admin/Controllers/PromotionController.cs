﻿using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.ProductCategory;
using PetStoreProject.Repositories.Promotion;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PromotionController : Controller
    {
        private readonly IBrandRepository _brand;
        private readonly IProductCategoryRepository _productCategory;
        private readonly IPromotionRepository _promotion;

        public PromotionController(IBrandRepository brand, IProductCategoryRepository productCategory, IPromotionRepository promotion)
        {
            _brand = brand;
            _productCategory = productCategory;
            _promotion = promotion;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Create()
        {
            var brand = _brand.GetBrands();
            var productCategory = _productCategory.GetProductCategories(null, false);
            ViewData["Brands"] = brand;
            ViewData["ProductCategories"] = productCategory;
            return View();
        }

        [HttpPost]
        public JsonResult Create(PromotionCreateRequest promotion)
        {
            _promotion.CreatePromotion(promotion);
            return Json("OK");
        }

        public IActionResult List()
        {
            var promotions = _promotion.GetPromotions();
            ViewData["Promotions"] = promotions;
            return View();
        }
    }
}
