using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Accounts
{
    public interface IAccountRepository
    {
        public Account getAccount(string email, string password);

        public bool checkEmailExist(string email);

        public void addNewCustomer(RegisterViewModel registerInfor);

        public void resetPassword(ResetPasswordViewModel resetPasswordVM);
    }
}
