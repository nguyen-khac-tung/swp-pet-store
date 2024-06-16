using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.OrderService
{
    public interface IOrderServiceRepository
    {
        public List<OrderServicesDetailViewModel> GetOrderServicesByCondition(OrderServiceModel orderServiceModel);

    }
}
