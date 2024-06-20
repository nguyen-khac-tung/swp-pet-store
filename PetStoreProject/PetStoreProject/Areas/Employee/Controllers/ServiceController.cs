using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Repositories.Service;
using PetStoreProject.ViewModels;
using PetStoreProject.Helpers;
using PetStoreProject.Models;

namespace PetStoreProject.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _service;

        public ServiceController(IServiceRepository service)
        {
            _service = service;
        }

        public IActionResult List()
        {
            ViewData["Services"] = _service.GetListServices();
            var orderServicePending = new OrderedServiceViewModel() { Status = "Chưa xác nhận" };
            ViewData["listOrderedPending"] = _service.GetOrderedServicesByConditions(orderServicePending, 1, 5);
            var totalPendingItems = _service.GetTotalCountOrderedServicesByConditions(orderServicePending);
            ViewData["numberPageOfListPending"] = (int)Math.Ceiling((double)totalPendingItems / 5);

            var orderServiceConfirmed = new OrderedServiceViewModel() { Status = "Đã xác nhận" };
            ViewData["listOrderedConfirmed"] = _service.GetOrderedServicesByConditions(orderServiceConfirmed, 1, 5);
            var totalConfirmedItems = _service.GetTotalCountOrderedServicesByConditions(orderServiceConfirmed);
            ViewData["numberPageOfListConfirmed"] = (int)Math.Ceiling((double)totalConfirmedItems / 5);

            var orderServicePaid = new OrderedServiceViewModel() { Status = "Đã thanh toán" };
            ViewData["listOrderedPaid"] = _service.GetOrderedServicesByConditions(orderServicePaid, 1, 5);
            var totalPaidItems = _service.GetTotalCountOrderedServicesByConditions(orderServicePaid);
            ViewData["numberPageOfListPaid"] = (int)Math.Ceiling((double)totalPaidItems / 5);
            return View();
        }

        [HttpPost]
        public Object OrderedSeviceByConditions(OrderedServiceViewModel orderServiceVM, int pageIndex, int pageSize)
        {
            var orderedServices = _service.GetOrderedServicesByConditions(orderServiceVM, pageIndex, pageSize);

            var totalItems = _service.GetTotalCountOrderedServicesByConditions(orderServiceVM);

            var numberPage = (int)Math.Ceiling((double)totalItems / 5);

            return Json(new
            {
                orderedServices = orderedServices,
                numberPage = numberPage
            });
        }

        [HttpGet]
        public IActionResult OrderServiceDetail(int orderServiceId)
        {
            var orderService = _service.GetOrderServiceDetail(orderServiceId);
            ViewData["WorkingTime"] = _service.GetWorkingTime(orderService.ServiceId);
            ViewData["Services"] = _service.GetListServices();
            ViewData["PetTypes"] = _service.GetFistServiceOption(orderService.ServiceId).PetTypes;
            ViewData["Weights"] = _service.GetFirstServiceAndListWeightOfPetType(orderService.ServiceId, orderService.PetType).Weights;
            return View(orderService);
        }

        [HttpPost]
        public ActionResult OrderServiceDetail(BookServiceViewModel orderServiceInfo)
        {
            ViewData["WorkingTime"] = _service.GetWorkingTime(orderServiceInfo.ServiceId);
            ViewData["Services"] = _service.GetListServices();
            ViewData["PetTypes"] = _service.GetFistServiceOption(orderServiceInfo.ServiceId).PetTypes;
            ViewData["Weights"] = _service.GetFirstServiceAndListWeightOfPetType(orderServiceInfo.ServiceId, orderServiceInfo.PetType).Weights;
            if (ModelState.IsValid)
            {
                bool isPhoneValid = PhoneNumber.isValid(orderServiceInfo.Phone);
                if (isPhoneValid == false)
                {
                    ViewBag.PhoneMess = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                    return View(orderServiceInfo);
                }

                _service.UpdateOrderService(orderServiceInfo);
                ViewData["UpdateSuccess"] = $"Thông tin lịch hẹn của " +
                    $"<strong style='font-size: 16px;'>{orderServiceInfo.Name}</strong> " +
                    $"đã được cập nhật thành công.";
                return View(orderServiceInfo);
            }
            else
            {
                return View(orderServiceInfo);
            }
        }

        [HttpGet]
        public IActionResult AddNewOrderService(string statusCreation)
        {
            var firstServiceOption = _service.GetFistServiceOption(1);
            ViewData["WorkingTime"] = _service.GetWorkingTime(1);
            ViewData["Services"] = _service.GetListServices();
            ViewData["PetTypes"] = firstServiceOption.PetTypes;
            ViewData["Weights"] = firstServiceOption.Weights;

            var orderService = new BookServiceViewModel();
            orderService.ServiceOptionId = firstServiceOption.ServiceOptionId;
            orderService.ServiceId = firstServiceOption.ServiceId;
            orderService.Price = firstServiceOption.Price.ToString("#,###") + " VND";

            ViewBag.Status = "PendingOrder";
            if (statusCreation == "Confirmed")
            {
                ViewBag.Status = "ConfirmedOrder";
                orderService.OrderDate = DateTime.Today.ToString("dd/MM/yyyy");
                orderService.OrderTime = roundOrderTime();
                orderService.Status = "Đã xác nhận";
            }

            return View(orderService);
        }

        [HttpPost]
        public IActionResult AddNewOrderService(BookServiceViewModel orderServiceInfo, string statusCreation)
        {
            ViewBag.Status = "PendingOrder";
            if (statusCreation == "Confirmed")
            {
                ViewBag.Status = "ConfirmedOrder";
            }
            ViewData["WorkingTime"] = _service.GetWorkingTime(orderServiceInfo.ServiceId);
            ViewData["Services"] = _service.GetListServices();
            ViewData["PetTypes"] = _service.GetFistServiceOption(orderServiceInfo.ServiceId).PetTypes;
            ViewData["Weights"] = _service.GetFirstServiceAndListWeightOfPetType(orderServiceInfo.ServiceId, orderServiceInfo.PetType).Weights;
            if (ModelState.IsValid)
            {
                bool isPhoneValid = PhoneNumber.isValid(orderServiceInfo.Phone);
                if (isPhoneValid == false)
                {
                    ViewBag.PhoneMess = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                    return View(orderServiceInfo);
                }

                _service.AddOrderService(orderServiceInfo);
                ViewData["CreateSuccess"] = $"Thông tin lịch hẹn của " +
                    $"<strong style='font-size: 16px;'>{orderServiceInfo.Name}</strong> " +
                    $"đã được tạo mới thành công.";
                return View(orderServiceInfo);
            }
            else
            {
                return View(orderServiceInfo);
            }
        }

        [HttpGet]
        public ServiceOptionViewModel GetServiceOptionByChangeService(int serviceId)
        {
            return _service.GetFistServiceOption(serviceId);
        }

        [HttpGet]
        public ServiceOptionViewModel GetServiceOptionByChangePetType(int serviceId, string petType)
        {
            return _service.GetFirstServiceAndListWeightOfPetType(serviceId, petType);
        }

        [HttpGet]
        public ServiceOptionViewModel GetServiceOptionByChangeWeight(int serviceId, string petType, string weight)
        {
            return _service.GetNewServiceOptionBySelectWeight(serviceId, petType, weight);
        }

        [HttpGet]
        public BookServiceViewModel UpdateStatusOrderService(int orderServiceId, string status)
        {
            _service.UpdateStatusOrderService(orderServiceId, status);
            return _service.GetOrderServiceDetail(orderServiceId);
        }

        [HttpGet]
        public void CancelOrderService(int orderServiceId)
        {
            _service.DeleteOrderService(orderServiceId);
        }

        public TimeOnly roundOrderTime()
        {
            TimeOnly roundTime;
            TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

            if (now.Minute == 0)
            {
                roundTime = now;
            }
            else if (now.Minute <= 30)
            {
                roundTime = new TimeOnly(now.Hour, 30);
            }
            else
            {
                roundTime = new TimeOnly(now.Hour + 1, 0);
            }
            return roundTime;
        }
    }
}
