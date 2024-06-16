using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Order
{
    public interface IOrderRepository
    {
        public List<OrderDetailViewModel> GetOrderDetailByCondition(OrderModel orderModel);
    }
}
