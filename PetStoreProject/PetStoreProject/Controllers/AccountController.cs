using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Helper;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepo)
        {
            _accountRepository = accountRepo;
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
                ViewBag.Mess = "Email hoặc mật khẩu không chính xác.";
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
    }
}
