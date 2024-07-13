using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ConsultionCreateRequestViewModel consultion)
        {
            if (ModelState.IsValid)
            {
                var consultionId = _consultionRepository.CreateConsultion(consultion);
                ViewData["Result"] = "Success";
            }
            return View();
        }
    }
}
