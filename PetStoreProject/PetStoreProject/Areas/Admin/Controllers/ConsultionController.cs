using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Consultion;

namespace PetStoreProject.Areas.Admin.Controllers
{
     [Area("Admin")]
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
        public IActionResult List()
        {
            var consultions = _consultionRepository.GetListConsultion();
            ViewData["consultion"] = consultions;
            return View();
        }
    }
}