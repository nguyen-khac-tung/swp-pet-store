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

            if (account.Password == null)
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
            Account account = new Account();
            account.Email = registerInfor.Email;
            if (registerInfor.Password != null)
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(registerInfor.Password);
            }
            else
            {
                account.Password = null;
            }

            Role role = _context.Roles.FirstOrDefault(r => r.RoleId == 3);

            List<Role> roles = new List<Role>();
            roles.Add(role);

            account.Roles = roles;

            _context.Accounts.Add(account);

            _context.SaveChanges();

            var accountId = (from a in _context.Accounts
                             where a.Email == registerInfor.Email
                             select a.AccountId).FirstOrDefault();

            var customer = new Customer
            {
                FullName = registerInfor.FullName,
                Phone = registerInfor.Phone,
                Email = registerInfor.Email,
                AccountId = accountId,
            };

            _context.Customers.Add(customer);
            
            _context.SaveChanges();
        }

        public void resetPassword(ResetPasswordViewModel resetPasswordVM)
        {
            var account = _context.Accounts.FirstOrDefault(acc => acc.Email == resetPasswordVM.Email);

            if (account != null)
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordVM.Password);

                _context.SaveChanges();
            }
        }

        public string? getOldPassword(string email)
        {
            var password = (from a in _context.Accounts
                            where a.Email == email
                            select a.Password).FirstOrDefault();
            return password;
        }

        public void changePassword(ChangePasswordViewModel changePasswordVM)
        {
            var account = (from a in _context.Accounts
                           where a.Email == changePasswordVM.Email
                           select a).FirstOrDefault();

            account.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordVM.NewPassword);

            _context.SaveChanges();
        }

        public List<string> GetUserRoles(string email)
        {
            var roles = (from a in _context.Accounts
                         join ar in _context.AccountRoles on a.AccountId equals ar.AccountId
                         join r in _context.Roles on ar.RoleId equals r.RoleId
                         where a.Email == email
                         select r.Name).ToList();

            return roles;
        }

        public string GetUserName(string email, string userRole)
        {
            string? userName;
            if (userRole == "Admin")
            {
                userName = (from a in _context.Accounts
                            join ad in _context.Admins on a.AccountId equals ad.AccountId
                            where a.Email == email
                            select ad.FullName).FirstOrDefault();
            }
            else if (userRole == "Employee")
            {
                userName = (from a in _context.Accounts
                            join e in _context.Employees on a.AccountId equals e.AccountId
                            where a.Email == email
                            select e.FullName).FirstOrDefault();
            }
            else
            {
                userName = (from a in _context.Accounts
                            join c in _context.Customers on a.AccountId equals c.AccountId
                            where a.Email == email
                            select c.FullName).FirstOrDefault();
            }
            return userName;

        public List<AccountDetailViewModel> GetAccountEmployees()
        {
            var accountEmployees = (from e in _context.Employees
                                    join a in _context.Accounts on e.AccountId equals a.AccountId
                                    select new AccountDetailViewModel
                                    {
                                        AccountId = e.AccountId,
                                        UserId = e.EmployeeId,
                                        FullName = e.FullName,
                                        Phone = e.Phone,
                                        Email = e.Email,
                                        Gender = e.Gender,
                                        Roles = a.Roles.ToList()
                                    }).ToList();
            return accountEmployees;
        }

        public List<AccountDetailViewModel> GetAccountCustomers()
        {
            var accountCustomers = (from c in _context.Customers
                                    join a in _context.Accounts on c.AccountId equals a.AccountId
                                    select new AccountDetailViewModel
                                    {
                                        AccountId = c.AccountId,
                                        UserId = c.CustomerId,
                                        FullName = c.FullName,
                                        Phone = c.Phone,
                                        Email = c.Email,
                                        Gender = c.Gender,
                                        Roles = a.Roles.ToList()
                                    }).ToList();
            return accountCustomers;
        }

        public List<AccountDetailViewModel> GetAccountAdmins()
        {
            var accountAdmins = (from ad in _context.Admins
                                 join a in _context.Accounts on ad.AccountId equals a.AccountId
                                 select new AccountDetailViewModel
                                 {
                                     AccountId = ad.AccountId,
                                     UserId = ad.AdminId,
                                     FullName = ad.FullName,
                                     Phone = ad.Phone,
                                     Email = ad.Email,
                                     Gender = ad.Gender,
                                     Roles = a.Roles.ToList()
                                 }).ToList();
            return accountAdmins;
        }

        public List<AccountDetailViewModel> GetAccounts(int userType)
        {
            List<AccountDetailViewModel> accounts = new List<AccountDetailViewModel>();
            if (userType == 0)
            {
                accounts.AddRange(GetAccountEmployees());
                accounts.AddRange(GetAccountCustomers());
                accounts.AddRange(GetAccountAdmins());
            }
            else
            {
                if (userType == 2)
                {
                    accounts.AddRange(GetAccountAdmins());
                }
                else if (userType == 3)
                {
                    accounts.AddRange(GetAccountEmployees());
                }
                else
                {
                    accounts.AddRange(GetAccountCustomers());
                }
            }

            return accounts;
        }
    }
}
