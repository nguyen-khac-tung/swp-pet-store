using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
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

            _context.Accounts.Add(account);

            _context.SaveChanges();

            var accountId = (from a in _context.Accounts
                             where a.Email == registerInfor.Email
                             select a.AccountId).FirstOrDefault();

            AccountRole accountRole = new AccountRole()
            {
                RoleId = 3,
                AccountId = accountId
            };

            _context.AccountRoles.Add(accountRole);

            _context.SaveChanges();

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
        }

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
                                        DoB = e.DoB,
                                        Address = e.Address.Trim(),
                                        Email = e.Email,
                                        Gender = e.Gender,
                                        Roles = _context.AccountRoles.Where(ar => ar.AccountId == e.AccountId).Select(ar => ar.Role).ToList()
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
                                        DoB = c.DoB,
                                        Address = c.Address.Trim(),
                                        Email = c.Email,
                                        Gender = c.Gender,
                                        Roles = _context.AccountRoles.Where(ar => ar.AccountId == c.AccountId).Select(ar => ar.Role).ToList()
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
                                     DoB = ad.DoB,
                                     Address = ad.Address.Trim(),
                                     Email = ad.Email,
                                     Gender = ad.Gender,
                                     Roles = _context.AccountRoles.Where(ar => ar.AccountId == ad.AccountId).Select(ar => ar.Role).ToList()
                                 }).ToList();
            return accountAdmins;
        }

        public List<AccountDetailViewModel> GetAccounts(int userType, int selectedRole, string searchName)
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

            if (selectedRole != 0)
            {
                accounts = accounts.Where(acc => acc.Roles.Any(role => role.RoleId == selectedRole)).ToList();
            }
            if (searchName != "")
            {
                accounts = accounts.Where(acc => acc.FullName.ToLower().Contains(searchName.ToLower())).ToList();
            }

            return accounts;
        }

        public int UpdateRoleAccount(int accountId, List<int> roleAccounts)
        {
            int status = 0;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Retrieve the old roles associated with the account
                    var roleOlds = _context.AccountRoles
                        .Where(ar => ar.AccountId == accountId)
                        .Select(ar => ar.RoleId)
                        .ToList();

                    // Use HashSet for faster lookup
                    var roleAccountsSet = new HashSet<int>(roleAccounts);
                    var roleOldsSet = new HashSet<int>(roleOlds);

                    // Roles to remove: old roles that are not in the new roles
                    var rolesToRemove = roleOldsSet.Except(roleAccountsSet).ToList();
                    // Roles to add: new roles that are not in the old roles
                    var rolesToAdd = roleAccountsSet.Except(roleOldsSet).ToList();

                    // Remove roles
                    if (rolesToRemove.Any())
                    {
                        foreach (var roleId in rolesToRemove)
                        {
                            var roleToRemove = _context.AccountRoles
                                .FirstOrDefault(ar => ar.AccountId == accountId && ar.RoleId == roleId);

                            if (roleToRemove != null)
                            {
                                _context.AccountRoles.Remove(roleToRemove);
                            }
                        }
                        _context.SaveChanges();
                    }

                    // Add roles
                    if (rolesToAdd.Any())
                    {
                        foreach (var roleId in rolesToAdd)
                        {
                            _context.AccountRoles.Add(new AccountRole
                            {
                                AccountId = accountId,
                                RoleId = roleId
                            });
                        }
                        _context.SaveChanges();
                    }

                    transaction.Commit();
                    status = 1;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }

                return status;
            }
        }

    }
}
