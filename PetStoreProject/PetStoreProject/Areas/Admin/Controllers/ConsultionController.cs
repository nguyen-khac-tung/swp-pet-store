using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Consultion;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ConsultionController : Controller
    {
        private readonly IConsultionRepository _consultion;
        private readonly PetStoreDBContext _context;
        public ConsultionController(IConsultionRepository consultion, PetStoreDBContext context)
        {
            _consultion = consultion;
            _context = context;
        }
        [HttpGet]
        public IActionResult List()
        {
            var consultions = _consultion.GetListConsultion();
            ViewData["consultion"] = consultions;
            return View();
        }

        [HttpPost]
        public JsonResult Detail(int consultionId)
        {
            var c = _consultion.GetDetail(consultionId);
            return Json(c);
        }

        [HttpPost]
        public JsonResult Reply(int consultionId, string messagge)
        {
            _consultion.Reply(consultionId, messagge);
            return Json(new { success = true });
        }
    }
}