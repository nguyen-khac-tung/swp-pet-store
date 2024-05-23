using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Accounts
{
    public interface IAccountRepository
    {
        public Account getAccount(string email, string password);
    }
}
