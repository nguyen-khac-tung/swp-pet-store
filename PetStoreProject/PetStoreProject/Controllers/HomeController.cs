using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;

namespace PetStoreProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly PetStoreDBContext _context;
        public HomeController(PetStoreDBContext dbContext) {
            _context = dbContext;
        }
        public IActionResult Index(string? success)
        {   
            if(success != null) {
                ViewBag.Success = success;
            }
            return View();
        }
    }
}
