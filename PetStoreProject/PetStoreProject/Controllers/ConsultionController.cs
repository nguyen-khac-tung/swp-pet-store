using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Consultion;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Controllers
{
    public class ConsultionController : Controller
    {
        private readonly IConsultionRepository _consultionRepository;
        private readonly PetStoreDBContext _context;

        public ConsultionController(IConsultionRepository consultionRepository, PetStoreDBContext context)
        {
            _consultionRepository = consultionRepository;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var formconsultion = new ConsultionViewModels();
            ViewData["formconsultion"] = formconsultion;
            return View(formconsultion);
        }

        [HttpPost]
        public JsonResult Create(CreateConsultion consultion)
        {
            var date = DateTime.Now;
            var consultionId = _consultionRepository.CreateConsultion(consultion);
            return Json(new { consultionId = consultionId });
        }
    }
}
