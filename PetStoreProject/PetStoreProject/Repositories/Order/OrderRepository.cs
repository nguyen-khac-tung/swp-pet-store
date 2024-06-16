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

        public List<OrderDetailViewModel> GetOrderDetailByCondition(OrderModel orderModel)
        {
            var orders = GetOrderDetailByCustomerId(orderModel.UserId);

            if (int.TryParse(orderModel.SearchOrderId, out int searchOrderId))
            {
                orders = orders.Where(o => o.OrderId == searchOrderId).ToList();
            }

            if(orderModel.SearchName != null)
            {
                orders = orders.Where(o => o.FullName.ToLower().Contains(orderModel.SearchName.ToLower())).ToList();
            }

            if (orderModel.SearchDate != DateOnly.MinValue)
            {
                orders = orders.Where(o => DateOnly.FromDateTime(o.OrderDate) == orderModel.SearchDate).ToList();
            }

            if (orderModel.SortOrderId == "abc")
                orders = orders.OrderBy(o => o.OrderId).ToList();
            else if (orderModel.SortOrderId == "zxy")
                orders = orders.OrderByDescending(o => o.OrderId).ToList();

            if (orderModel.SortName == "abc")
                orders = orders.OrderBy(o => o.FullName).ToList();
            else if (orderModel.SortName == "zxy")
                orders = orders.OrderByDescending(o => o.FullName).ToList();

            if (orderModel.SortTotalItems == "abc")
                orders = orders.OrderBy(o => o.totalOrderItems).ToList();
            else if (orderModel.SortTotalItems == "zxy")
                orders = orders.OrderByDescending(o => o.totalOrderItems).ToList();

            if (orderModel.SortDate == "abc")
                orders = orders.OrderBy(o => o.OrderDate).ToList();
            else if (orderModel.SortDate == "zxy")
                orders = orders.OrderByDescending(o => o.OrderDate).ToList();

            if (orderModel.SortPrice == "abc")
                orders = orders.OrderBy(o => o.TotalAmount).ToList();
            else if (orderModel.SortPrice == "zxy")
                orders = orders.OrderByDescending(o => o.TotalAmount).ToList();

            return orders;
        }
    }
}
