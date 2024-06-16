using PetStoreProject.Areas.Employee.ViewModels;

namespace PetStoreProject.Repositories.Order
{
    public interface IOrderRepository
    {
        public List<OrderDetailViewModel> GetOrderDetailByCondition(int userId, string orderId,
            string name, string date, string totalItems, string price, string search);

        public int GetOrderDetailCount(int userId);
    }
}
