using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Filters;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Admin;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _account;
        private readonly EmailService _emailService;
        private readonly IAdminRepository _admin;

        public AccountController(IAccountRepository account, EmailService emailService, IAdminRepository admin)
        {
            _account = account;
            _emailService = emailService;
            _admin = admin;
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

        [HttpGet]
        public IActionResult ProfileAccount()
        {
            //var email = HttpContext.Session.GetString("userEmail");

            var email = "admin1@gmail.com";
            var admin = _admin.GetAdmin(email);
            if (admin == null)
            {
                return NotFound("Admin not found.");
            }
            var adminViewModel = new UserViewModel
            {
                UserId = admin.AdminId,
                FullName = admin.FullName,
                DoB = admin.DoB,
                Gender = admin.Gender,
                Phone = admin.Phone,
                Address = admin.Address,
                Email = email,
                AccountId = admin.AccountId,
                RoleName = "Quản trị viên"
            };
            return View("_ProfileUser",adminViewModel);
        }

        [HttpPost]
        public IActionResult ProfileAccount(UserViewModel admin)
        {

            if (ModelState.IsValid)
            {

                //var oldEmail = HttpContext.Session.GetString("userEmail");
                var oldEmail = "admin@gmail.com";
                if (oldEmail != admin.Email)
                {
                    bool isEmailExist = _account.CheckEmailExist(admin.Email);
                    if (isEmailExist)
                    {
                        ViewBag.EmailMess = "Địa chỉ email này đã được liên kết với một tài khoản khác. Vui lòng nhập một email khác.";
                        return View("_ProfileUser", admin);
                    }
                }

                bool isPhoneValid = PhoneNumber.isValid(admin.Phone);
                if (isPhoneValid == false)
                {
                    ViewBag.PhoneMess = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                    return View("_ProfileUser", admin);
                }

                if(admin.Address == null)
                {
                    ViewBag.Address = "Địa chỉ không hợp lệ. Vui lòng nhập lại";
                    return View("_ProfileUser", admin);
                }

                if(admin.DoB >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
                {
                    ViewBag.DoB = "Ngày sinh phải trước ngày hiện tại";
                    return View("_ProfileUser", admin);
                }

                HttpContext.Session.SetString("userEmail", admin.Email);
                HttpContext.Session.SetString("userName", admin.FullName);
                _admin.UpdateProfileAdmin(admin);
                return View("_ProfileUser", admin);
            }
            else
            {
                return View("_ProfileUser", admin);
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            //var email = HttpContext.Session.GetString("userEmail");
            var email = "admin1@fpt.edu.vn";
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
                return View("_ChangePasswordUser", ChangePasswordVM);
            }
            else
            {
                return View("_ChangePasswordUser", ChangePasswordVM);
            }
        }
    }
}
