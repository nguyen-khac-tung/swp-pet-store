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
                totalAccount = totalAccount,
                currentPage = pageIndexLocal,
                pageSize = pageSizeLocal,
                numberPage = numberPage
            });
        }

        [HttpGet]
        public IActionResult CustomerDetail(int userId)
        {
            var account = _account.GetAccountCustomers(userId);

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

            ViewBag.SearchOrderId = orderServiceModel.SearchOrderServiceId;
            ViewBag.SearchName = orderServiceModel.SearchName;
            ViewBag.SearchDate = orderServiceModel.SearchDate;
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
            var email = HttpContext.Session.GetString("userEmail");

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
                var errors = new Dictionary<string, string>();

                var oldEmail = HttpContext.Session.GetString("userEmail");
                if (oldEmail != employee.Email)
                {
                    bool isEmailExist = _account.CheckEmailExist(employee.Email);
                    if (isEmailExist)
                    {
                        errors["Email"] = "Địa chỉ email này đã được liên kết với một tài khoản khác. Vui lòng nhập một email khác.";
                    }
                }

                bool isPhoneValid = PhoneNumber.isValid(employee.Phone);
                if (isPhoneValid == false)
                {
                    errors["Phone"] = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                }

                if (employee.Address == null)
                {
                    errors["Address"] = "Địa chỉ không hợp lệ. Vui lòng nhập lại.";
                }

                if (employee.DoB >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
                {
                    errors["DoB"] = "Ngày sinh phải trước ngày hiện tại.";
                }

                if (errors.Any())
                {
                    return new JsonResult(new { isSuccess = false, errors });
                }

                HttpContext.Session.SetString("userEmail", employee.Email);
                HttpContext.Session.SetString("userName", employee.FullName);
                _employee.UpdateProfileEmployee(employee);
                return new JsonResult(new { isSuccess = true, message = "Cập nhật thành công", updatedData = employee });
            }
            else
            {
                var modelStateErrors = ModelState
                                        .Where(ms => ms.Value.Errors.Any())
                                        .ToDictionary(
                                            ms => ms.Key,
                                            ms => ms.Value.Errors.FirstOrDefault()?.ErrorMessage ?? "Lỗi không xác định"
                                        );
                return new JsonResult(new { isSuccess = false, errors = modelStateErrors });
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            var email = HttpContext.Session.GetString("userEmail");
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
                return new JsonResult(new { success = true, message = "Thay đổi mật khẩu thành công." });
            }
            else
            {
                return View("_ChangePasswordUser", ChangePasswordVM);
            }
        }

    }
}
