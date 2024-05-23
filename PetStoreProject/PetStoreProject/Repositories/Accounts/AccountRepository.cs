using PetStoreProject.Models;
using PetStoreProject.ViewModels;

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

            if (isPasswordValid == false)
            {
                return null;
            }
            else
            {
                return account;
            }
        }

        public bool checkEmailExist(string email)
        {
            var account = (from acc in _context.Accounts
                           where acc.Email == email
                           select acc).FirstOrDefault();

            if (account != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void addNewCustomer(RegisterViewModel registerInfor)
        {
            var account = new Account
            {
                Email = registerInfor.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerInfor.Password),
            };
            _context.Accounts.Add(account);

            var customer = new Customer
            {
                FullName = registerInfor.FullName,
                Phone = registerInfor.Phone,
                Email = registerInfor.Email,
            };
            _context.Customers.Add(customer);

            _context.SaveChanges();
        }
    }
}
