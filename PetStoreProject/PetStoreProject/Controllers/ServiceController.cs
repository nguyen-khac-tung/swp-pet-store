using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Service;

namespace PetStoreProject.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _service;

        public ServiceController(IServiceRepository service) {
            _service = service;
        }

        public IActionResult List()
        {
            var services = _service.GetListServices();
            return View(services);
        }
    }
}
