using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Filters;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.Service;
using PetStoreProject.ViewModels;
using System.Globalization;
using System.Security.Claims;

namespace PetStoreProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _account;
        private readonly EmailService _emailService;
        private readonly ICustomerRepository _customer;
        private readonly IServiceRepository _service;

        public AccountController(IAccountRepository accountRepo, EmailService emailService, ICustomerRepository customer,
            IServiceRepository service)
        {
            _account = accountRepo;
            _emailService = emailService;
            _customer = customer;
            _service = service;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Account account)
        {
            Account acc = _account.GetAccount(account.Email, account.Password);
            if (acc != null)
            {
                HttpContext.Session.SetString("userEmail", acc.Email);
                var role = _account.GetUserRoles(acc.Email);
                if (role == "Admin")

                {
                    var userName = _account.GetUserName(acc.Email, "Admin");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else if (role == "Employee")
                {
                    var userName = _account.GetUserName(acc.Email, "Employee");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("Index", "Home", new { area = "Employee" });
                }
                else
                {
                    var userName = _account.GetUserName(acc.Email, "Customer");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Mess"] = "Email hoặc mật khẩu không chính xác.";
                ViewBag.Email = account.Email;
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel registerInfor)
        {
            if (ModelState.IsValid)
            {
                bool isEmailExist = _account.CheckEmailExist(registerInfor.Email);
                bool isPhoneValid = PhoneNumber.isValid(registerInfor.Phone);
                if (isEmailExist)
                {
                    ViewBag.EmailMess = "Email này đã được liên kết với một tài khoản khác. " +
                        "Vui lòng đăng nhập hoặc sử dụng một email khác.";
                    return View();
                }
                else if (isPhoneValid == false)
                {
                    ViewBag.PhoneMess = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                    return View();
                }
                else
                {
                    _account.AddNewCustomer(registerInfor);
                    HttpContext.Session.SetString("userEmail", registerInfor.Email);
                    HttpContext.Session.SetString("userName", registerInfor.FullName);
                    return RedirectToAction("Index", "Home", new { success = "True" });
                }
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            bool isExistEmail = _account.CheckEmailExist(email);
            if (isExistEmail)
            {
                ViewBag.SuccessMess = "Yêu cầu đặt lại mật khẩu đã được gửi đến email của bạn. " +
                    "Nếu không nhận được email, vui lòng nhập lại địa chỉ email và gửi lại yêu cầu.";

                var resetLink = Url.Action("ResetPassword", "Account", new { userEmail = email }, Request.Scheme);

                var subject = "Đặt lại mật khẩu cho tài khoản khách hàng";

                var body = "<h2 style=\"font-weight:normal;font-size:24px;margin:0 0 10px\">Đặt lại mật khẩu</h2>" +
                    "<p style=\"color:#777;line-height:150%;font-size:16px;margin:0\">Đặt lại mật khẩu cho tài khoản khách hàng " +
                    "tại Animart Store. Nếu bạn không cần đặt mật khẩu mới, bạn có thể yên tâm xóa email này đi.</p> <br>" +
                    $"<p style=\"font-size:20px; margin:10px 0 0;\">Hãy bấm vào <a href=\"{resetLink}\">đặt lại mật khẩu</a></p>";

                _emailService.SendEmail(email, subject, body);
                return View();
            }
            else
            {
                ViewBag.ErrorMess = "Email không hợp lệ. Vui lòng nhập lại địa chỉ email.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string userEmail)
        {
            var ResetPasswordVM = new ResetPasswordViewModel { Email = userEmail };
            return View(ResetPasswordVM);
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel ResetPasswordVM)
        {
            if (ModelState.IsValid)
            {
                _account.ResetPassword(ResetPasswordVM);
                HttpContext.Session.SetString("userEmail", ResetPasswordVM.Email);
                var role = _account.GetUserRoles(ResetPasswordVM.Email);
                if (role == "Admin")
                {
                    var userName = _account.GetUserName(ResetPasswordVM.Email, "Admin");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else if (role == "Employee")
                {
                    var userName = _account.GetUserName(ResetPasswordVM.Email, "Employee");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("Index", "Home", new { area = "Employee" });
                }
                else
                {
                    var userName = _account.GetUserName(ResetPasswordVM.Email, "Customer");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View(ResetPasswordVM);
            }
        }

        public IActionResult LoginGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse"),
                Items = { { "prompt", "select_account" } },
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public IActionResult GoogleFailure()
        {
            TempData["Mess"] = "Đăng nhập tài khoản Google không thành công. Vui lòng thử lại.";
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (result.Succeeded)
            {
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                var userInfo = claims.ToDictionary(claim => claim.Type, claim => claim.Value);

                var fullName = userInfo[ClaimTypes.Name];
                var email = userInfo[ClaimTypes.Email];

                bool isEmailExist = _account.CheckEmailExist(email);
                if (isEmailExist)
                {
                    HttpContext.Session.SetString("userEmail", email);
                    var role = _account.GetUserRoles(email);
                    if (role == "Admin")
                    {
                        var userName = _account.GetUserName(email, "Admin");
                        HttpContext.Session.SetString("userName", userName);
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (role == "Employee")
                    {
                        var userName = _account.GetUserName(email, "Employee");
                        HttpContext.Session.SetString("userName", userName);
                        return RedirectToAction("Index", "Home", new { area = "Employee" });
                    }
                    else
                    {
                        var userName = _account.GetUserName(email, "Customer");
                        HttpContext.Session.SetString("userName", userName);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    var resgister = new RegisterViewModel { FullName = fullName, Email = email };
                    _account.AddNewCustomer(resgister);
                    HttpContext.Session.SetString("userEmail", email);
                    HttpContext.Session.SetString("userName", fullName);
                    return RedirectToAction("Index", "Home", new { success = "True" });
                }
            }

            TempData["Mess"] = "Đăng nhập tài khoản Google không thành công. Vui lòng thử lại.";
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied(string[] allowedRoles)
        {
            List<string> roles = allowedRoles.ToList();
            return View(roles);
        }


        [RoleAuthorize("Customer")]
        public IActionResult Profile()
        {
            return View();
        }


        [RoleAuthorize("Customer")]
        [HttpGet]
        public IActionResult ProfileDetail()
        {
            var email = HttpContext.Session.GetString("userEmail");
            var customer = _customer.GetCustomer(email);
            var customerVM = new CustomerViewModel
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Gender = customer.Gender,
                DoB = customer.DoB?.ToString("dd/MM/yyyy"),
                Address = customer.Address,
                Phone = customer.Phone,
                Email = customer.Email,
                AccountId = customer.AccountId,
            };
            return View(customerVM);
        }


        [RoleAuthorize("Customer")]
        [HttpPost]
        public IActionResult ProfileDetail(CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                var oldEmail = HttpContext.Session.GetString("userEmail");
                if (oldEmail != customer.Email)
                {
                    bool isEmailExist = _account.CheckEmailExist(customer.Email);
                    if (isEmailExist)
                    {
                        ViewBag.EmailMess = "Địa chỉ email này đã được liên kết với một tài khoản khác. Vui lòng nhập một email khác.";
                        return View(customer);
                    }
                }
                bool isPhoneValid = PhoneNumber.isValid(customer.Phone);
                if (isPhoneValid == false)
                {
                    ViewBag.PhoneMess = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                    return View(customer);
                }

                HttpContext.Session.SetString("userEmail", customer.Email);
                HttpContext.Session.SetString("userName", customer.FullName);
                _customer.UpdateProfile(customer);
                return View(customer);
            }
            else
            {
                return View(customer);
            }
        }


        [RoleAuthorize("Customer")]
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
            return View(ChangePasswordVM);
        }


        [RoleAuthorize("Customer")]
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
                    return View(ChangePasswordVM);
                }
            }

            if (ModelState.IsValid)
            {
                _account.ChangePassword(ChangePasswordVM);
                return View(ChangePasswordVM);
            }
            else
            {
                return View(ChangePasswordVM);
            }
        }


        [RoleAuthorize("Customer")]
        [HttpGet]
        public ActionResult OrderServiceHistory()
        {
            var email = HttpContext.Session.GetString("userEmail");
            var customer = _customer.GetCustomer(email);
            var orderedServices = _service.GetOrderedServicesOfCustomer(customer.CustomerId);
            return View(orderedServices);
        }

        [RoleAuthorize("Customer")]
        [HttpGet]
        public ActionResult OrderServiceDetail(int orderServiceId)
        {
            var orderService = _service.GetOrderServiceDetail(orderServiceId);
            ViewData["WorkingTime"] = _service.GetWorkingTime(orderService.ServiceId);
            ViewData["Services"] = _service.GetListServices();
            ViewData["PetTypes"] = _service.GetFistServiceOption(orderService.ServiceId).PetTypes;
            ViewData["Weights"] = _service.GetFirstServiceAndListWeightOfPetType(orderService.ServiceId, orderService.PetType).Weights;
            return View(orderService);
        }

        [RoleAuthorize("Customer")]
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
                ViewData["UpdateSuccess"] = "Cửa hàng ANIMART đã nhận được đặt hẹn của bạn và sẽ sớm liên hệ với bạn để xác nhận. Cảm ơn bạn đã tin tưởng và đặt lịch dịch vụ của chúng tôi!";
                return View(orderServiceInfo);
            }
            else
            {
                return View(orderServiceInfo);
            }
        }

        [RoleAuthorize("Customer")]
        [HttpGet]
        public ServiceOptionViewModel GetServiceOptionByChangeService(int serviceId)
        {
            return _service.GetFistServiceOption(serviceId);
        }

        [RoleAuthorize("Customer")]
        [HttpGet]
        public ServiceOptionViewModel GetServiceOptionByChangePetType(int serviceId, string petType)
        {
            return _service.GetFirstServiceAndListWeightOfPetType(serviceId, petType);
        }

        [RoleAuthorize("Customer")]
        [HttpGet]
        public ServiceOptionViewModel GetServiceOptionByChangeWeight(int serviceId, string petType, string weight)
        {
            return _service.GetNewServiceOptionBySelectWeight(serviceId, petType, weight);
        }

        [RoleAuthorize("Customer")]
        [HttpGet]
        public void CancelOrderService (int orderServiceId)
        {
            _service.DeleteOrderService(orderServiceId);
        }
    }
}
