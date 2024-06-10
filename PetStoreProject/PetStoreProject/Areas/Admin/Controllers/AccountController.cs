using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.ViewModels;
using PetStoreProject.Helper;
using System.Linq;
using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _account;
        private readonly EmailService _emailService;

        public AccountController(IAccountRepository account, EmailService emailService)
        {
            _account = account;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public IActionResult List(int? pageIndex, int? pageSize, int? selectRole, string? searchName, string? sortName, string? selectStatus)
        {
            var pageIndexLocal = pageIndex ?? 1;

            var pageSizeLocal = pageSize ?? 10;

            var accounts = _account.GetAccounts(2, searchName ?? "", selectStatus ?? "");

            var numberPage = (int)Math.Ceiling(accounts.Count / (double)(pageSizeLocal));

            if (sortName == "abc")
            {
                accounts = accounts.OrderBy(a => a.FullName).ToList();
            }
            else if (sortName == "zxy")
            {
                accounts = accounts.OrderByDescending(a => a.FullName).ToList();
            }

            accounts = accounts.Skip((pageIndexLocal - 1) * pageSizeLocal).Take(pageSizeLocal).ToList();


            return new JsonResult(new
            {
                accounts = accounts,
                currentPage = pageIndexLocal,
                pageSize = pageSizeLocal,
                numberPage = numberPage
            });
        }

        [HttpPost]
        public IActionResult AddAccount(AccountViewModel accountViewModel)
        {
            if(ModelState.IsValid)
            {

            }

            return View();
        }
    }
}
