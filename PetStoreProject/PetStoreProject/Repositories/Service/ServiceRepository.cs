﻿using Microsoft.IdentityModel.Tokens;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;
using System.Globalization;

namespace PetStoreProject.Repositories.Service
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly PetStoreDBContext _context;

        public ServiceRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public List<ServiceViewModel> GetListServices()
        {
            var services = (from s in _context.Services
                            where s.IsDelete == false
                            select new ServiceViewModel
                            {
                                ServiceId = s.ServiceId,
                                Name = s.Name,
                                Type = s.Type,
                                SupDescription = s.SupDescription,
                            }).ToList();

            foreach (var service in services)
            {
                var Image = _context.Images.Where(i => i.ServiceId == service.ServiceId).FirstOrDefault();
                service.ImageUrl = Image.ImageUrl;
                var serviceOption = (from s in _context.Services
                                     join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                                     where s.ServiceId == service.ServiceId
                                     orderby so.Price ascending
                                     select so).FirstOrDefault();
                service.Price = serviceOption.Price;
            }

            return services;
        }

        public ServiceDetailViewModel GetServiceDetail(int serviceId)
        {
            var service = (from s in _context.Services
                           where s.ServiceId == serviceId
                           select new ServiceDetailViewModel
                           {
                               ServiceId = s.ServiceId,
                               Name = s.Name,
                               Type = s.Type,
                               Description = s.Description,
                               IsDelete = s.IsDelete,
                           }).FirstOrDefault();

            var images = (from s in _context.Services
                          join i in _context.Images on s.ServiceId equals i.ServiceId
                          where s.ServiceId == serviceId
                          select i).ToList();

            service.Images = images;
            return service;
        }

        public ServiceOptionViewModel GetFistServiceOption(int serviceId)
        {
            var listPetType = (from s in _context.Services
                               join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                               where s.ServiceId == serviceId
                               select so.PetType).Distinct().ToList();

            var petType = listPetType.FirstOrDefault();

            var firstServiceOption = GetFirstServiceAndListWeightOfPetType(serviceId, petType);

            firstServiceOption.PetTypes = listPetType;

            return firstServiceOption;
        }

        public ServiceOptionViewModel GetFirstServiceAndListWeightOfPetType(int serviceId, string petType)
        {
            var firstServiceOption = (from so in _context.ServiceOptions
                                      where so.ServiceId == serviceId && so.PetType == petType
                                      select so).FirstOrDefault();
            var listWeight = (from so in _context.ServiceOptions
                              where so.ServiceId == serviceId && so.PetType == petType
                              orderby so.ServiceOptionId ascending
                              select so.Weight).ToList();
            return new ServiceOptionViewModel
            {
                ServiceId = firstServiceOption.ServiceId,
                ServiceOptionId = firstServiceOption.ServiceOptionId,
                PetType = firstServiceOption.PetType,
                Weight = firstServiceOption.Weight,
                Price = firstServiceOption.Price,
                Weights = listWeight
            };
        }

        public ServiceOptionViewModel GetNewServiceOptionBySelectWeight(int serviceId, string petType, string weight)
        {
            var serviceOption = (from so in _context.ServiceOptions
                                 where so.ServiceId == serviceId && so.PetType == petType && so.Weight == weight
                                 select new ServiceOptionViewModel
                                 {
                                     ServiceOptionId = so.ServiceOptionId,
                                     Price = so.Price,
                                 }).FirstOrDefault();

            return serviceOption;
        }

        public BookServiceViewModel GetBookingServiceInFo(int serviceOptionId)
        {
            var bookService = (from s in _context.Services
                               join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                               where so.ServiceOptionId == serviceOptionId
                               select new BookServiceViewModel
                               {
                                   ServiceOptionId = so.ServiceOptionId,
                                   ServiceId = so.ServiceId,
                                   ServiceName = s.Name,
                                   PetType = so.PetType,
                                   Weight = so.Weight,
                                   Price = so.Price.ToString("#,###") + "VND",
                               }).FirstOrDefault();

            return bookService;
        }

        public List<TimeOnly> GetWorkingTime(int serviceId)
        {
            var workingTime = (from s in _context.Services
                               join ts in _context.TimeServices on s.ServiceId equals ts.ServiceId
                               join wt in _context.WorkingTimes on ts.WorkingTimeId equals wt.WorkingTimeId
                               where s.ServiceId == serviceId
                               select wt.Time).ToList();
            return workingTime;
        }

        public void AddOrderService(BookServiceViewModel bookServiceInfo)
        {
            DateTime ordDate = DateTime.ParseExact(bookServiceInfo.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateOnly orderDate = DateOnly.FromDateTime(ordDate);
            var orderServiceDuplicate = (from os in _context.OrderServices
                                         where os.Name == bookServiceInfo.Name
                                         && os.Phone == bookServiceInfo.Phone
                                         && os.OrderDate == orderDate
                                         && os.OrderTime == bookServiceInfo.OrderTime
                                         && os.ServiceOptionId == bookServiceInfo.ServiceOptionId
                                         && os.Status == "Chưa xác nhận"
                                         select os).FirstOrDefault();
            if (orderServiceDuplicate != null)
            {
                return;
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Models.OrderService orderService = new Models.OrderService();
                    orderService.CustomerId = bookServiceInfo?.CustomerId;
                    orderService.Name = bookServiceInfo.Name;
                    orderService.Phone = bookServiceInfo.Phone;
                    orderService.OrderDate = orderDate;

                    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                    orderService.DateCreated = today;

                    orderService.OrderTime = bookServiceInfo.OrderTime;
                    orderService.ServiceOptionId = bookServiceInfo.ServiceOptionId;
                    orderService.Message = bookServiceInfo?.Message;

                    if (bookServiceInfo.Status.IsNullOrEmpty())
                    {
                        orderService.Status = "Chưa xác nhận";
                    }
                    else
                    {
                        orderService.Status = bookServiceInfo.Status;
                    }

                    orderService.IsDelete = false;

                    _context.Add(orderService);

                    _context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        public List<ServiceViewModel> GetOtherServices(int serviceId)
        {
            var services = (from s in _context.Services
                            where s.IsDelete == false && s.ServiceId != serviceId
                            select new ServiceViewModel
                            {
                                ServiceId = s.ServiceId,
                                Name = s.Name,
                                Type = s.Type,
                                SupDescription = s.SupDescription,
                            }).ToList();

            foreach (var service in services)
            {
                var Image = _context.Images.Where(i => i.ServiceId == service.ServiceId).FirstOrDefault();
                service.ImageUrl = Image.ImageUrl;
                var serviceOption = (from s in _context.Services
                                     join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                                     where s.ServiceId == service.ServiceId
                                     orderby so.Price ascending
                                     select so).FirstOrDefault();
                service.Price = serviceOption.Price;
            }

            return services;
        }

        public List<BookServiceViewModel> GetOrderedServicesOfCustomer(int customerId)
        {
            var orderedServices = (from os in _context.OrderServices
                                   join so in _context.ServiceOptions on os.ServiceOptionId equals so.ServiceOptionId
                                   join s in _context.Services on so.ServiceId equals s.ServiceId
                                   where os.CustomerId == customerId
                                   select new BookServiceViewModel
                                   {
                                       OrderServiceId = os.OrderServiceId,
                                       ServiceName = s.Name,
                                       DateCreated = os.DateCreated.ToString("dd/MM/yyyy"),
                                       OrderDate = os.OrderDate.ToString("dd/MM/yyyy"),
                                       Status = os.Status
                                   }).ToList();
            return orderedServices;
        }

        public BookServiceViewModel GetOrderServiceDetail(int orderServiceId)
        {
            var orderServiceDetail = (from os in _context.OrderServices
                                      join so in _context.ServiceOptions on os.ServiceOptionId equals so.ServiceOptionId
                                      join s in _context.Services on so.ServiceId equals s.ServiceId
                                      where os.OrderServiceId == orderServiceId
                                      select new BookServiceViewModel
                                      {
                                          OrderServiceId = os.OrderServiceId,
                                          ServiceId = s.ServiceId,
                                          ServiceOptionId = os.ServiceOptionId,
                                          Name = os.Name,
                                          Phone = os.Phone,
                                          OrderDate = os.OrderDate.ToString("dd/MM/yyyy"),
                                          DateCreated = os.DateCreated.ToString("dd/MM/yyyy"),
                                          OrderTime = os.OrderTime,
                                          ServiceName = s.Name,
                                          PetType = so.PetType,
                                          Weight = so.Weight,
                                          Price = so.Price.ToString("#,###") + " VND",
                                          Message = os.Message,
                                          Status = os.Status,
                                      }).FirstOrDefault();

            var employeeId = _context.OrderServices.Where(os => os.OrderServiceId == orderServiceId).FirstOrDefault()?.EmployeeId;
            if (employeeId != null)
            {
                orderServiceDetail.EmployeeName = _context.Employees.Where(e => e.EmployeeId == employeeId).FirstOrDefault()?.FullName;
            }

            return orderServiceDetail;
        }

        public void UpdateOrderService(BookServiceViewModel orderService)
        {
            var order = (from os in _context.OrderServices
                         where os.OrderServiceId == orderService.OrderServiceId
                         select os).FirstOrDefault();

            order.Name = orderService.Name;
            order.Phone = orderService.Phone;

            DateTime orderDate = DateTime.ParseExact(orderService.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            order.OrderDate = DateOnly.FromDateTime(orderDate);

            order.OrderTime = orderService.OrderTime;

            order.ServiceOptionId = orderService.ServiceOptionId;

            order.Message = orderService.Message;

            _context.SaveChanges();
        }

        public void DeleteOrderService(int orderServiceId)
        {
            var orderService = (from os in _context.OrderServices
                                where os.OrderServiceId == orderServiceId
                                select os).FirstOrDefault();
            orderService.Status = "Đã hủy";

            orderService.IsDelete = true;

            _context.SaveChanges();
        }

        public void UpdateStatusOrderService(int orderServiceId, string status, int employeeId)
        {
            var orderService = (from os in _context.OrderServices
                                where os.OrderServiceId == orderServiceId
                                select os).FirstOrDefault();
            orderService.Status = status;

            if(employeeId != 0)
            {
                orderService.EmployeeId = employeeId;
            }

            _context.SaveChanges();
        }

        public List<BookServiceViewModel> GetOrderedServicesByConditions(OrderedServiceViewModel orderServiceVM,
            int pageIndex, int pageSize)
        {
            List<BookServiceViewModel> orderedServices = new List<BookServiceViewModel>();

            // Determine the list of order services that satisfy the filter options
            DateOnly? orderServiceDate = null;
            if (DateTime.TryParseExact(orderServiceVM.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                orderServiceDate = DateOnly.FromDateTime(parsedDate);
            }

            string? customerNameOrPhone = orderServiceVM.NameOrPhone.IsNullOrEmpty() ? null : orderServiceVM.NameOrPhone;
            int? serviceId = orderServiceVM.ServiceId.IsNullOrEmpty() ? null : Int32.Parse(orderServiceVM.ServiceId);

            orderedServices = (from os in _context.OrderServices
                               join so in _context.ServiceOptions on os.ServiceOptionId equals so.ServiceOptionId
                               join s in _context.Services on so.ServiceId equals s.ServiceId
                               where os.Status == orderServiceVM.Status
                               && (customerNameOrPhone == null || os.Name.Contains(customerNameOrPhone) || os.Phone.Contains(customerNameOrPhone))
                               && (serviceId == null || s.ServiceId == serviceId) //If serviceId is null, then return true and do not evaluate the subsequent condition.
                               && (orderServiceDate == null || os.OrderDate == orderServiceDate)
                               select new BookServiceViewModel
                               {
                                   OrderServiceId = os.OrderServiceId,
                                   Name = os.Name,
                                   Phone = os.Phone,
                                   OrderDate = os.OrderDate.ToString("dd/MM/yyyy"),
                                   OrderTime = os.OrderTime,
                                   ServiceName = s.Name,
                                   PetType = so.PetType,
                                   Status = os.Status,
                               }).ToList();
            // -----------------------------

            // Sort the list according to different criteria depending on the sorting parameter.
            if (orderServiceVM.SortServiceId != null)
            {
                if (orderServiceVM.SortServiceId == "Ascending")
                    orderedServices = orderedServices.OrderBy(o => o.OrderServiceId).ToList();
                else
                    orderedServices = orderedServices.OrderByDescending(o => o.OrderServiceId).ToList();
            }

            if (orderServiceVM.SortCustomerName != null)
            {
                if (orderServiceVM.SortCustomerName == "Ascending")
                    orderedServices = orderedServices.OrderBy(o => o.Name).ToList();
                else
                    orderedServices = orderedServices.OrderByDescending(o => o.Name).ToList();
            }

            if (orderServiceVM.SortOrderDate != null)
            {
                if (orderServiceVM.SortOrderDate == "Ascending")
                    orderedServices = orderedServices.OrderBy(o =>
                    DateTime.ParseExact(o.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToList();
                else
                    orderedServices = orderedServices.OrderByDescending(o =>
                    DateTime.ParseExact(o.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToList();
            }

            if (orderServiceVM.SortOrderTime != null)
            {
                if (orderServiceVM.SortOrderTime == "Ascending")
                    orderedServices = orderedServices.OrderBy(o => o.OrderTime).ToList();
                else
                    orderedServices = orderedServices.OrderByDescending(o => o.OrderTime).ToList();
            }
            //-----------------------------

            //Skip the number of elements depending on the index of the current page and the page size.
            var orderedServicesDisplay = orderedServices.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            //------------------------------

            return orderedServicesDisplay;
        }

        public int GetTotalCountOrderedServicesByConditions(OrderedServiceViewModel orderServiceVM)
        {
            DateOnly? orderServiceDate = null;
            if (DateTime.TryParseExact(orderServiceVM.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                orderServiceDate = DateOnly.FromDateTime(parsedDate);
            }

            string? customerNameOrPhone = orderServiceVM.NameOrPhone.IsNullOrEmpty() ? null : orderServiceVM.NameOrPhone;
            int? serviceId = orderServiceVM.ServiceId.IsNullOrEmpty() ? null : Int32.Parse(orderServiceVM.ServiceId);

            var orderedServices = (from os in _context.OrderServices
                                   join so in _context.ServiceOptions on os.ServiceOptionId equals so.ServiceOptionId
                                   join s in _context.Services on so.ServiceId equals s.ServiceId
                                   where os.Status == orderServiceVM.Status
                                   && (customerNameOrPhone == null || os.Name.Contains(customerNameOrPhone) || os.Phone.Contains(customerNameOrPhone))
                                   && (serviceId == null || s.ServiceId == serviceId)  //If serviceId is null, then return true and do not evaluate the subsequent condition.
                                   && (orderServiceDate == null || os.OrderDate == orderServiceDate)
                                   select new BookServiceViewModel
                                   {
                                       OrderServiceId = os.OrderServiceId,
                                   }).ToList();

            var totalItem = orderedServices.Count;
            return totalItem;
        }
    }
}
