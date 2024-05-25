using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Helper;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.ViewModels;
using System.Security.Claims;

namespace PetStoreProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly EmailService _emailService;

        public AccountController(IAccountRepository accountRepo, EmailService emailService)
        {
            _accountRepository = accountRepo;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Account account)
        {
            Account acc = _accountRepository.getAccount(account.Email, account.Password);
            if (acc != null)
            {
                HttpContext.Session.SetString("Account", acc.Email);
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
                bool isEmailExist = _accountRepository.checkEmailExist(registerInfor.Email);
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
                    _accountRepository.addNewCustomer(registerInfor);
                    HttpContext.Session.SetString("Account", registerInfor.Email);
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
            bool isExistEmail = _accountRepository.checkEmailExist(email);
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
                _accountRepository.resetPassword(resetPasswordVM);
                HttpContext.Session.SetString("Account", resetPasswordVM.Email);
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
                var passwordDefault = "SigninGoogle";

                bool isEmailExist = _accountRepository.checkEmailExist(email);
                if (isEmailExist)
                {
                    HttpContext.Session.SetString("Account", email);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var resgister = new RegisterViewModel { FullName = fullName, Email = email, Password = passwordDefault };
                    _accountRepository.addNewCustomer(resgister);
                    HttpContext.Session.SetString("Account", email);
                    return RedirectToAction("Index", "Home", new { success = "True" });
                }
            }

            TempData["Mess"] = "Đăng nhập tài khoản Google không thành công. Vui lòng thử lại.";
            return RedirectToAction("Login", "Account");
        }

    }
}
