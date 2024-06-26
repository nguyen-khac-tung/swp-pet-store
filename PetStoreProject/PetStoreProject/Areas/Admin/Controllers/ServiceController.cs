﻿using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Repositories.Service;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _service;

        public ServiceController(IServiceRepository service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult List()
        {
            var serviceFilterVM = new ServiceFilterViewModel();
            ViewData["ListServiceType"] = _service.GetListServiceTypes();
            ViewData["ListService"] = _service.GetListServiceByConditions(serviceFilterVM, 1, 7);
            var totalService = _service.GetTotalCountListServicesByConditions(serviceFilterVM);
            ViewData["numberOfPage"] = (int)Math.Ceiling((double)totalService / 7);
            return View();
        }

        [HttpPost]
        public Object List(ServiceFilterViewModel serviceFilterVM, int pageIndex, int pageSize)
        {
            var listService = _service.GetListServiceByConditions(serviceFilterVM, pageIndex, pageSize);

            var totalService = _service.GetTotalCountListServicesByConditions(serviceFilterVM);

            var numberOfPage = (int)Math.Ceiling((double)totalService / 7);

            return Json(new
            {
                listService = listService,
                numberOfPage = numberOfPage
            });
        }
    }
}
