using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Accounts;

namespace PetStoreProject.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _account;

        public AccountController(IAccountRepository account)
        {
            _account = account;
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
    }
}
