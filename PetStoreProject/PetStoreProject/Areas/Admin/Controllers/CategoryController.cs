using AspNetCore;
using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Attribute;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.Category;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.ProductCategory;
using PetStoreProject.Repositories.Size;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _category;
        private readonly ICloudinaryService _cloudinary;
        
        public CategoryController(ICategoryRepository category, ICloudinaryService cloudinary)
        {
            _category = category;
            _cloudinary = cloudinary;
        }
        [HttpGet]
        public IActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> fetchCategory(int? categoryId, string? key, bool? sortName, bool? isDelete, int pageNumber = 1, int pageSize = 10)
        {
            pageSize = Math.Min(pageSize, 30);
            List<Category> listCategory = await _category.GetCategories(pageNumber, pageSize, categoryId, key, sortName, isDelete);
            return new JsonResult(new
            {
                listCategory = listCategory,
                currentPage = pageNumber,
                pageSize = pageSize,
                numberPage = (int)Math.Ceiling((double)listCategory.Count / pageSize)
            });
        }
    }
}

