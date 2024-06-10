﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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

        public Account GetAccount(string email, string password)
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

        public bool CheckEmailExist(string email)
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

        public void AddNewCustomer(RegisterViewModel registerInfor)
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

            account.RoleId = 3;

            account.IsDelete = false;

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

        public void ResetPassword(ResetPasswordViewModel ResetPasswordVM)
        {
            var account = _context.Accounts.FirstOrDefault(acc => acc.Email == ResetPasswordVM.Email);

            if (account != null)
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(ResetPasswordVM.Password);

                _context.SaveChanges();
            }
        }

        public string? GetOldPassword(string email)
        {
            var password = (from a in _context.Accounts
                            where a.Email == email
                            select a.Password).FirstOrDefault();
            return password;
        }

        public void ChangePassword(ChangePasswordViewModel ChangePasswordVM)
        {
            var account = (from a in _context.Accounts
                           where a.Email == ChangePasswordVM.Email
                           select a).FirstOrDefault();

            account.Password = BCrypt.Net.BCrypt.HashPassword(ChangePasswordVM.NewPassword);

            _context.SaveChanges();
        }

        public List<string> GetUserRoles(string email)
        {
            var roles = (from a in _context.Accounts
                         join r in _context.Roles on a.RoleId equals r.RoleId
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
                                        Role = _context.Roles.FirstOrDefault(r => r.RoleId == a.RoleId),
                                        IsDelete = e.IsDelete
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
                                        Role = _context.Roles.FirstOrDefault(r => r.RoleId == a.RoleId),
                                        IsDelete = c.IsDelete
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
                                     Role = _context.Roles.FirstOrDefault(r => r.RoleId == a.RoleId),
                                     IsDelete = ad.IsDelete
                                 }).ToList();
            return accountAdmins;
        }

        public List<AccountDetailViewModel> GetAccounts(int userType, string searchName, string selectedStatus)
        {
            List<AccountDetailViewModel> accounts = new List<AccountDetailViewModel>();
            switch (userType)
            {
                case 1:
                    accounts = GetAccountAdmins();
                    break;
                case 2:
                    accounts = GetAccountEmployees();
                    break;
                case 3:
                    accounts = GetAccountCustomers();
                    break;
            }

            if (searchName != "")
            {
                accounts = accounts.Where(acc => acc.FullName.ToLower().Contains(searchName.ToLower())).ToList();
            }

            if (selectedStatus == "0")
            {
                accounts = accounts.Where(acc => acc.IsDelete == false).ToList();
            }
            else if (selectedStatus == "1")
            {
                accounts = accounts.Where(acc => acc.IsDelete == true).ToList();
            }

            return accounts;
        }
    }
}
