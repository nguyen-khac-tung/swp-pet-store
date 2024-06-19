using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Employee;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.OrderService;
using PetStoreProject.ViewModels;
using System.Security.Cryptography.X509Certificates;
using PetStoreProject.Helpers;

namespace PetStoreProject.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _account;
        private readonly IOrderRepository _order;
        private readonly IOrderServiceRepository _orderService;
        private readonly IEmployeeRepository _employee;

        public AccountController(IAccountRepository account, IOrderRepository order, IOrderServiceRepository service, IEmployeeRepository employee)
        {
            _account = account;
            _order = order;
            _orderService = service;
            _employee = employee;
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

            var accounts = _account.GetAccountCustomers(pageIndexLocal, pageSizeLocal, 3, searchName ?? "", sortName ?? "", selectStatus ?? "");

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
        [HttpGet]
        public IActionResult ProfileAccount()
        {
            //var email = HttpContext.Session.GetString("userEmail");

            var email = "duongnkhe171810@fpt.edu.vn";
            var employee = _employee.GetEmployee(email);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }
            var employeeViewModel = new UserViewModel
            {
                UserId = employee.EmployeeId,
                FullName = employee.FullName,
                DoB = employee.DoB,
                Gender = employee.Gender,
                Phone = employee.Phone,
                Address = employee.Address,
                Email = email,
                AccountId = employee.AccountId,
                RoleName = "Nhân viên"
            };
            return View("_ProfileUser", employeeViewModel);
        }

        [HttpPost]
        public IActionResult ProfileAccount(UserViewModel employee)
        {

            if (ModelState.IsValid)
            {

                //var oldEmail = HttpContext.Session.GetString("userEmail");
                var oldEmail = "duongnkhe171810@fpt.edu.vn";
                if (oldEmail != employee.Email)
                {
                    bool isEmailExist = _account.CheckEmailExist(employee.Email);
                    if (isEmailExist)
                    {
                        ViewBag.EmailMess = "Địa chỉ email này đã được liên kết với một tài khoản khác. Vui lòng nhập một email khác.";
                        return View("_ProfileUser", employee);
                    }
                }

                bool isPhoneValid =  PhoneNumber.isValid(employee.Phone);
                if (isPhoneValid == false)
                {
                    ViewBag.PhoneMess = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                    return View("_ProfileUser", employee);
                }

                if (employee.Address == null)
                {
                    ViewBag.Address = "Địa chỉ không hợp lệ. Vui lòng nhập lại";
                    return View("_ProfileUser", employee);
                }

                if (employee.DoB >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
                {
                    ViewBag.DoB = "Ngày sinh phải trước ngày hiện tại";
                    return View("_ProfileUser", employee);
                }

                HttpContext.Session.SetString("userEmail", employee.Email);
                HttpContext.Session.SetString("userName", employee.FullName);
                _employee.UpdateProfileEmployee(employee);
                return View("_ProfileUser", employee);
            }
            else
            {
                return View("_ProfileUser", employee);
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            //var email = HttpContext.Session.GetString("userEmail");
            var email = "duongnkhe171810@fpt.edu.vn";
            var ChangePasswordVM = new ChangePasswordViewModel { Email = email };
            string? oldPassword = _account.GetOldPassword(email);
            if (oldPassword != null)
            {
                ChangePasswordVM.OldPassword = oldPassword;
            }
            else
            {
                ChangePasswordVM.OldPassword = null;
            }
            return View("_ChangePasswordUser", ChangePasswordVM);
        }


        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel ChangePasswordVM)
        {
            if (ChangePasswordVM.OldPassword != null)
            {
                var passwordStored = _account.GetOldPassword(ChangePasswordVM.Email);
                bool isValid = BCrypt.Net.BCrypt.Verify(ChangePasswordVM.OldPassword, passwordStored);
                if (isValid == false)
                {
                    ViewBag.Message = "Mật khẩu cũ không chính xác. Vui lòng thử lại.";
                    return View("_ChangePasswordUser", ChangePasswordVM);
                }
            }

            if (ModelState.IsValid)
            {
                _account.ChangePassword(ChangePasswordVM);
                return View("_ChangePasswordUser", ChangePasswordVM);
            }
            else
            {
                return View("_ChangePasswordUser", ChangePasswordVM);
            }
        }
    }
}
