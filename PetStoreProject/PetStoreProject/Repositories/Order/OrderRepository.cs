using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PetStoreDBContext _context;

        public OrderRepository(PetStoreDBContext dBContext)
        {
            _context = dBContext;
        }
        public List<OrderDetailViewModel> GetOrderDetailByCustomerId(int customerId)
        {
            var orders = (from o in _context.Orders
                          where o.CustomerId == customerId
                          select new OrderDetailViewModel
                          {
                              CustomerId = o.CustomerId,
                              Email = o.Email,
                              FullName = o.FullName,
                              Phone = o.Phone,
                              ShipAddress = o.ShipAddress,
                              OrderDate = o.OrderDate,
                              TotalAmount = o.TotalAmount,
                              PaymentMethod = o.PaymetMethod,
                              OrderId = o.OrderId,
                              totalOrderItems = _context.OrderItems.Count(ot => ot.OrderId == o.OrderId),
                          }).ToList();

            return orders;
        }
        public int GetOrderDetailCount(int userId)
        {
            int count = 0;

            var orders = GetOrderDetailByCustomerId(userId);

            count = orders.Count;

            return count;
        }

        public List<OrderDetailViewModel> GetOrderDetailByCondition(int userId, string orderId, string name, string date, string totalItems, string price, string search)
        {
            var orders = GetOrderDetailByCustomerId(userId);

            if (int.TryParse(search, out int searchOrderId))
            {
                orders = orders.Where(o => o.OrderId == searchOrderId).ToList();
            }

            if (orderId == "abc")
                orders = orders.OrderBy(o => o.OrderId).ToList();
            else if (orderId == "zxy")
                orders = orders.OrderByDescending(o => o.OrderId).ToList();

            if (name == "abc")
                orders = orders.OrderBy(o => o.FullName).ToList();
            else if (name == "zxy")
                orders = orders.OrderByDescending(o => o.FullName).ToList();

            if (totalItems == "abc")
                orders = orders.OrderBy(o => o.totalOrderItems).ToList();
            else if (totalItems == "zxy")
                orders = orders.OrderByDescending(o => o.totalOrderItems).ToList();

            if (date == "abc")
                orders = orders.OrderBy(o => o.OrderDate).ToList();
            else if (date == "zxy")
                orders = orders.OrderByDescending(o => o.OrderDate).ToList();

            if (price == "abc")
                orders = orders.OrderBy(o => o.TotalAmount).ToList();
            else if (price == "zxy")
                orders = orders.OrderByDescending(o => o.TotalAmount).ToList();

            return orders;
        }
    }
}
