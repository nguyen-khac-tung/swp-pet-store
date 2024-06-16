using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Category;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _category;

        public CategoryController(ICategoryRepository category)
        {
            _category = category;
        }
        [HttpGet]
        public IActionResult List()
        {
            var categories = _category.GetListCategory();
            ViewData["categories"] = categories;
            return View();
        }

        [HttpPost]
        public JsonResult Create(string CategoryName)
        {
            var cateId = _category.CreateCategory(CategoryName);
            return Json(new { cateId = cateId });
        }

    }
}

