﻿using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Service
{
    public interface IServiceRepository
    {
        public List<ServiceViewModel> GetListServices();

        public List<int> GetAllServiceId();

        public ServiceDetailViewModel GetServiceDetail(int serviceId);

        public List<ServiceOptionViewModel> GetServiceOptions(int serviceId);

        public ServiceOptionViewModel GetFistServiceOption(int serviceId);

        public ServiceOptionViewModel GetFirstServiceAndListWeightOfPetType(int serviceId, string petType);
        
        public ServiceOptionViewModel GetNewServiceOptionBySelectWeight(int serviceId, string petType, string weight);

        public BookServiceViewModel GetBookingServiceInFo(int serviceOptionId);

        public List<TimeOnly> GetWorkingTime(int serviceId);

        public List<TimeOnly> GetWorkingTimeByDate(string date);

        public List<TimeOnly> GetWorkingTimeByDateForUpdate(string date, TimeOnly orderTime);

        public void AddOrderService(BookServiceViewModel bookServiceInfo);

        public List<ServiceViewModel> GetOtherServices(int serviceId);

        public List<BookServiceViewModel> GetOrderedServicesOfCustomer(int customerId);

        public BookServiceViewModel GetOrderServiceDetail(int orderServiceId);

        public void UpdateOrderService(BookServiceViewModel orderService);

        public void DeleteOrderService(int orderServiceId);

        public void UpdateStatusOrderService(int orderServiceId, string status, int employeeId);

        public List<BookServiceViewModel> GetOrderedServicesByConditions(OrderedServiceViewModel orderServiceVM,
             int pageIndex, int pageSize);

        public int GetTotalCountOrderedServicesByConditions(OrderedServiceViewModel orderServiceVM);

        public List<string> GetListServiceTypes();

        public List<ServiceTableViewModel> GetListServiceByConditions(ServiceFilterViewModel serviceFilterVM,
            int pageIndex, int pageSize);

        public int GetTotalCountListServicesByConditions(ServiceFilterViewModel serviceFilterVM);
    }
}
