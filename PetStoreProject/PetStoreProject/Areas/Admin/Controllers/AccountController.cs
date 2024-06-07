using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.ViewModels;
using PetStoreProject.Helper;
using System.Linq;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _account;

        public AccountController(IAccountRepository account)
        {
            _account = account;
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ListUsers(int? pageIndex, int? pageSize, int? selectedRole, string? searchName)
        {
            var pageIndexLocal = pageIndex ?? 1;

            var pageSizeLocal = pageSize ?? 10;

            var accounts = _account.GetAccounts(0, selectedRole ?? 0, searchName ?? "");

            var numberPage = (int)Math.Ceiling(accounts.Count / (double)(pageSizeLocal));

            accounts = accounts.Skip((pageIndexLocal - 1) * pageSizeLocal).Take(pageSizeLocal).ToList();

            return new JsonResult(new
            {
                accounts = accounts,
                currentPage = pageIndexLocal,
                pageSize = pageSizeLocal,
                numberPage = numberPage
            });
        }
    }
}
