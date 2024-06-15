using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;

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
        public IActionResult List(int? pageIndex, int? pageSize, string? searchName, string? sortName, string? selectStatus)
        {
            var pageIndexLocal = pageIndex ?? 1;

            var pageSizeLocal = pageSize ?? 10;

            var accounts = _account.GetAccounts(pageIndexLocal, pageSizeLocal, 2, searchName ?? "", sortName ?? "", selectStatus ?? "");

            var totalAccount = _account.GetAccountCount(2);

            var numberPage = (int)Math.Ceiling(totalAccount / (double)(pageSizeLocal));

            return new JsonResult(new
            {
                userType = 2,
                accounts = accounts,
                currentPage = pageIndexLocal,
                pageSize = pageSizeLocal,
                numberPage = numberPage
            });
        }

        [HttpPost]
        public IActionResult AddAccount(AccountViewModel accountViewModel)
        {
            if (ModelState.IsValid)
            {
                bool isExistEmail = _account.CheckEmailExist(accountViewModel.Email);
                bool isValidPhone = PhoneNumber.isValid(accountViewModel.Phone);

                if (isExistEmail)
                {
                    var emailMess = "Email này đã tồn tại trong hệ thống!\n Vui lòng sử dụng email khác để tạo tài khoản.";

                    return Json(new { success = false, error = emailMess });
                }
                else if (isValidPhone == false)
                {
                    var phoneMess = "Số điện thoại không hợp lệ! Vui lòng nhập lại.";

                    return Json(new { success = false, error = phoneMess });
                }
                else
                {
                    var password = GeneratePassword.GenerateAutoPassword(10);

                    var emailTitle = "Thông báo! Mật khẩu ứng dụng.";

                    var emailBody = "Mật khẩu: " + password;

                    _emailService.SendEmail(accountViewModel.Email, emailTitle, emailBody);

                    accountViewModel.Password = password;

                    _account.AddNewEmployment(accountViewModel);

                    return Json(new { success = true });
                }
            }
            else
            {
                var errors = ModelState.ToDictionary(kvp => kvp.Key,
                                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { success = false, errors = errors });
            }

        }

        [HttpPost]
        public IActionResult DeleteAccount(int accountId, string passwordAdmin)
        {
            var isExistAccount = _account.IsExistAccount(accountId);

            if (isExistAccount == false)
            {
                return Json(new { success = false, isExistAccout = false });
            }
            else
            {
                var emailAdmin = HttpContext.Session.GetString("userEmail");

                Account account = _account.GetAccount(emailAdmin, passwordAdmin);

                if (account == null)
                {
                    return Json(new { success = false, passwordAdmin = false });
                }
                else
                {
                    var status = _account.UpdateStatusDeleteEmployee(accountId);

                    return Json(new { success = status });
                }
            }
        }
    }
}
