using AdminVM = PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Accounts
{
    public interface IAccountRepository
    {
        public Account GetAccount(string email, string password);

        public bool CheckEmailExist(string email);

        public void AddNewCustomer(RegisterViewModel registerInfor);

        public void ResetPassword(ResetPasswordViewModel ResetPasswordVM);

        public string? GetOldPassword(string email);

        public string GetUserRoles(string email);

        public string GetUserName(string email, string userRole);

        public void ChangePassword(ChangePasswordViewModel ChangePasswordVM);

        public List<AccountDetailViewModel> GetAccounts(int pageIndex, int pageSize, int userType, string searchName, string sortName, string selectedStatus);

        public void AddNewEmployment(AdminVM.AccountViewModel accountViewModel);

        public bool IsExistAccount(int accountId); 

        public int UpdateStatusDeleteEmployee(int accountId);

        public int GetAccountCount(int userType);

        public AccountDetailViewModel? GetAccountByUserId(int userType, int userId);
    }
}
