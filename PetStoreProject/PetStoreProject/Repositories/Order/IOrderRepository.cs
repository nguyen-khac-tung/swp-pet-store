using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Order
{
    public interface IOrderRepository
    {
        public List<OrderDetailViewModel> GetOrderDetailByCondition(OrderModel orderModel);

        public int GetCountOrder(int customerId);

        public void AddOrder(Models.Order order);
    }
}
