using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Helper;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Customers;
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

        public AccountController(IAccountRepository accountRepo, EmailService emailService, ICustomerRepository customer)
        {
            _account = accountRepo;
            _emailService = emailService;
            _customer = customer;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Account account)
        {
            Account acc = _account.getAccount(account.Email, account.Password);
            if (acc != null)
            {
                var name = _customer.getCustomer(account.Email)?.FullName;
                HttpContext.Session.SetString("Account", acc.Email);
                HttpContext.Session.SetString("CustomerName", name ?? "");
                return RedirectToAction("Index", "Home");
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
                bool isEmailExist = _account.checkEmailExist(registerInfor.Email);
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
                    _account.addNewCustomer(registerInfor);
                    HttpContext.Session.SetString("Account", registerInfor.Email);
                    HttpContext.Session.SetString("CustomerName", registerInfor.FullName);
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
            bool isExistEmail = _account.checkEmailExist(email);
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
            var resetPasswordVM = new ResetPasswordViewModel { Email = userEmail };
            return View(resetPasswordVM);
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel resetPasswordVM)
        {
            if (ModelState.IsValid)
            {
                _account.resetPassword(resetPasswordVM);
                var name = _customer.getCustomer(resetPasswordVM.Email)?.FullName;
                HttpContext.Session.SetString("Account", resetPasswordVM.Email);
                HttpContext.Session.SetString("CustomerName", name ?? "");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(resetPasswordVM);
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

                bool isEmailExist = _account.checkEmailExist(email);
                if (isEmailExist)
                {
                    var name = _customer.getCustomer(email)?.FullName;
                    HttpContext.Session.SetString("Account", email);
                    HttpContext.Session.SetString("CustomerName", name ?? "");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var resgister = new RegisterViewModel { FullName = fullName, Email = email };
                    _account.addNewCustomer(resgister);
                    HttpContext.Session.SetString("Account", email);
                    HttpContext.Session.SetString("CustomerName", fullName);
                    return RedirectToAction("Index", "Home", new { success = "True" });
                }
            }

            TempData["Mess"] = "Đăng nhập tài khoản Google không thành công. Vui lòng thử lại.";
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ProfileDetail()
        {
            var email = HttpContext.Session.GetString("Account");
            var cusomer = _customer.getCustomer(email);
            var customerVM = new CustomerViewModel
            {
                CustomerId = cusomer.CustomerId,
                FullName = cusomer.FullName,
                Gender = cusomer.Gender,
                DoB = cusomer.DoB?.ToString("dd/MM/yyyy"),
                Address = cusomer.Address,
                Phone = cusomer.Phone,
                Email = cusomer.Email,
                AccountId = cusomer.AccountId,
            };
            return View(customerVM);
        }

        [HttpPost]
        public IActionResult ProfileDetail(CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                var oldEmail = HttpContext.Session.GetString("Account");
                if (oldEmail != customer.Email)
                {
                    bool isEmailExist = _account.checkEmailExist(customer.Email);
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

                HttpContext.Session.SetString("Account", customer.Email);
                HttpContext.Session.SetString("CustomerName", customer.FullName);
                _customer.UpdateProfile(customer);
                return View(customer);
            }
            else
            {
                return View(customer);
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            var email = HttpContext.Session.GetString("Account");
            var changePasswordVM = new ChangePasswordViewModel { Email = email };
            string? oldPassword = _account.getOldPassword(email);
            if (oldPassword != null)
            {
                changePasswordVM.OldPassword = oldPassword;
            }
            else
            {
                changePasswordVM.OldPassword = null;
            }
            return View(changePasswordVM);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel changePasswordVM)
        {
            if (changePasswordVM.OldPassword != null)
            {
                var passwordStored = _account.getOldPassword(changePasswordVM.Email);
                bool isValid = BCrypt.Net.BCrypt.Verify(changePasswordVM.OldPassword, passwordStored);
                if (isValid == false)
                {
                    ViewBag.Message = "Mật khẩu cũ không chính xác. Vui lòng thử lại.";
                    return View(changePasswordVM);
                }
            }

            if (ModelState.IsValid)
            {
                _account.changePawword(changePasswordVM);
                return View(changePasswordVM);
            }
            else
            {
                return View(changePasswordVM);
            }
        }
    }
}
