using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Service;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Controllers
{
    [Route("{controller}/{action}")]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _service;

        public ServiceController(IServiceRepository service)
        {
            _service = service;
        }

        public IActionResult List()
        {
            var services = _service.GetListServices();
            return View(services);
        }

        [HttpGet("{serviceId}")]
        public IActionResult Detail(int serviceId)
        {
            ViewData["ServiceDetail"] = _service.GetServiceDetail(serviceId);
            ViewData["FirstServiceOption"] = _service.GetFistServiceOption(serviceId);
            ViewData["OtherServices"] = _service.GetOtherServices(serviceId);
            return View();
        }

        [HttpPost]
        public ServiceOptionViewModel GetOptionViewModel(int serviceId, string petType)
        {
            return _service.GetFirstServiceAndListWeightOfPetType(serviceId, petType);
        }

        [HttpPost]
        public ServiceOptionViewModel GetServiceOptionPrice(int serviceId, string petType, string weight)
        {
            return _service.GetNewServiceOptionBySelectWeight(serviceId, petType, weight);
        }
    }
}
