using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;

namespace PetStoreProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepo) {
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
    }
}
