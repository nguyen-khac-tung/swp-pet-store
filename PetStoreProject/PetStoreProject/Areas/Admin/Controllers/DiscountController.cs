using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Discount;
using PetStoreProject.Repositories.DiscountType;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiscountController : Controller
    {
        private readonly IDiscountTypeRepository _discountType;
        private readonly IDiscountRepository _discount;

        public DiscountController(IDiscountRepository discount, IDiscountTypeRepository discountType)
        {
            _discountType = discountType;
            _discount = discount;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var discountType = _discountType.GetDiscountTypes();
            ViewData["discountType"] = discountType;
            return View();
        }

        [HttpPost]
        public JsonResult Create(Discount discount)
        {
            var result = _discount.Create(discount);
            return Json(result);
        }
    }
}
