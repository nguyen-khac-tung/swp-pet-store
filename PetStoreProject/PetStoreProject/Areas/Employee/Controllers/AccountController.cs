using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.OrderService;
using System.Security.Cryptography.X509Certificates;

namespace PetStoreProject.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _account;
        private readonly IOrderRepository _order;
        private readonly IOrderServiceRepository _orderService;

        public AccountController(IAccountRepository account, IOrderRepository order, IOrderServiceRepository service)
        {
            _account = account;
            _order = order;
            _orderService = service;
        }

        [HttpGet]
        public IActionResult ListCustomer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ListCustomer(int? pageIndex, int? pageSize, string? searchName, string? sortName, string? selectStatus)
        {
            var pageIndexLocal = pageIndex ?? 1;

            var pageSizeLocal = pageSize ?? 10;

            var accounts = _account.GetAccounts(pageIndexLocal, pageSizeLocal, 3, searchName ?? "", sortName ?? "", selectStatus ?? "");

            var totalAccount = _account.GetAccountCount(3);

            var numberPage = (int)Math.Ceiling((double)totalAccount / pageSizeLocal);


            return new JsonResult(new
            {
                userType = 3,
                accounts = accounts,
                currentPage = pageIndexLocal,
                pageSize = pageSizeLocal,
                numberPage = numberPage
            });
        }

        [HttpGet]
        public IActionResult CustomerDetail(int userId)
        {
            var account = _account.GetAccountByUserId(3, userId);

            return View(account);
        }

        [HttpPost]
        public IActionResult OrderHistory(OrderModel orderModel)
        {

            var orders = _order.GetOrderDetailByCondition(orderModel);

            var totalOrders = orders.Count;

            ViewBag.searchOrderId = orderModel.SearchOrderId;
            ViewBag.searchName = orderModel.SearchName;
            ViewBag.searchDateOrder = orderModel.SearchDate;

            ViewBag.totalOrders = totalOrders;

            string json = JsonConvert.SerializeObject(orders);

            ViewBag.json = json;

            return View(orders);
        }

        [HttpPost]
        public IActionResult OrderServiceHistory(OrderServiceModel orderServiceModel)
        {
            List<OrderServicesDetailViewModel> orderServices = _orderService.GetOrderServicesByCondition(orderServiceModel);

            var totalOrderServices = orderServices.Count;

            ViewBag.searchOrderId = orderServiceModel.SearchOrderServiceId;
            ViewBag.searchName = orderServiceModel.SearchName;
            ViewBag.searchDate = orderServiceModel.SearchDate;
            ViewBag.SearchTime = orderServiceModel.SearchTime;
            ViewBag.Status = orderServiceModel.Status;

            ViewBag.totalOrderServices = totalOrderServices;

            string json = JsonConvert.SerializeObject(orderServices);

            ViewBag.json = json;

            return View(orderServices);
        }


    }
}
