using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Accounts
{
    public class AccountRepository : IAccountRepository
    {
        private readonly PetStoreDBContext _context;

        public AccountRepository(PetStoreDBContext dbContext)
        {
            _context = dbContext;
        }

        public Account getAccount(string email, string password)
        {
            var account = (from acc in _context.Accounts
                       where acc.Email == email
                       select acc).FirstOrDefault();
            if (account == null)
            {
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, account.Password);

            if(isPasswordValid == false)
            {
                return null;
            }else
            {
                return account;
            }
        }
    }
}
