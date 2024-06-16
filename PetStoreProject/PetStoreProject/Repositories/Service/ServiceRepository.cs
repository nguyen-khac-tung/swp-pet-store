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

            //foreach (var service in services)
            //{
            //    var Image = _context.Images.Where(i => i.ServiceId == service.ServiceId).FirstOrDefault();
            //    service.ImageUrl = Image.ImageUrl;
            //    var serviceOption = (from s in _context.Services
            //                         join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
            //                         where s.ServiceId == service.ServiceId
            //                         orderby so.Price ascending
            //                         select so).FirstOrDefault();
            //    service.Price = serviceOption.Price;
            //}

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
                                         select os).FirstOrDefault();
            if(orderServiceDuplicate != null)
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
                    orderService.Status = "Chưa xác nhận";
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
    }
}
