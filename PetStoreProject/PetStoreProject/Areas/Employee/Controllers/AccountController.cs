using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        public IActionResult OrderHistory(int userId, string orderId, string name, string date, string totalItems, string price, string search)
        {

            var orders = _order.GetOrderDetailByCondition(userId, orderId, name, date, totalItems, price, search);

            var totalOrders = _order.GetOrderDetailCount(userId);

            return View(orders);
        }

        [HttpPost]
        public IActionResult OrderServiceHistory(OrderServiceModel orderServiceModel)
        {
            var orderServices = _orderService.GetOrderServicesByCondition(orderServiceModel);

            return View(orderServices);
        }


    }
}
