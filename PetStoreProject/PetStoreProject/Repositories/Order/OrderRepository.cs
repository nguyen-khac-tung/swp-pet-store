﻿using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.Repositories.ProductOption;

namespace PetStoreProject.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PetStoreDBContext _context;
        private readonly IProductOptionRepository _productOptionRepository;

        public OrderRepository(PetStoreDBContext dBContext, IProductOptionRepository productOptionRepository)
        {
            _context = dBContext;
            _productOptionRepository = productOptionRepository;
        }
        public List<OrderDetailViewModel> GetOrderDetailByCustomerId(int customerId)
        {
            var orders = new List<OrderDetailViewModel>();
            if (customerId > 0)
            {
                orders = (from o in _context.Orders
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
                              OrderId = o.OrderId.ToString(),
                              ConsigneeName = o.ConsigneeFullName,
                              ConsigneePhone = o.ConsigneePhone,
                              totalOrderItems = _context.OrderItems.Count(ot => ot.OrderId == o.OrderId),
                              DiscountId = o.DiscountId,
                              Status = o.Status,
                              ShippingFee = o.ShippingFee,
                              ReturnId = o.ReturnId
                          }).ToList();
            }
            else
            {
                orders = (from o in _context.Orders
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
                              OrderId = o.OrderId.ToString(),
                              ConsigneeName = o.ConsigneeFullName,
                              ConsigneePhone = o.ConsigneePhone,
                              totalOrderItems = _context.OrderItems.Count(ot => ot.OrderId == o.OrderId),
                              DiscountId = o.DiscountId,
                              Status = o.Status,
                              ShippingFee = o.ShippingFee,
                              ReturnId = o.ReturnId
                          }).ToList();
            }
            return orders;
        }

        public List<OrderDetailViewModel> GetOrderDetailByCondition(OrderModel orderModel)
        {
            var orders = GetOrderDetailExcuteCondition(orderModel);

            orders = orders.Skip((orderModel.pageIndex - 1) * orderModel.pageSize).Take(orderModel.pageSize).ToList();

            return orders;
        }

        public List<OrderDetailViewModel> GetOrderDetailExcuteCondition(OrderModel orderModel)
        {
            var orders = GetOrderDetailByCustomerId(orderModel.UserId);

            if (long.TryParse(orderModel.SearchOrderId, out long searchOrderId))
            {
                orders = orders.Where(o => o.OrderId.Equals(searchOrderId.ToString())).ToList();
            }

            if (orderModel.SearchName != null)
            {
                orders = orders.Where(o => o.FullName.ToLower().Contains(orderModel.SearchName.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(orderModel.SearchDate))
            {
                if (DateOnly.TryParse(orderModel.SearchDate, out DateOnly searchDate))
                {
                    orders = orders.Where(o => DateOnly.FromDateTime(o.OrderDate) == searchDate).ToList();
                }
            }

            if (orderModel.SearchConsigneeName != null)
            {
                orders = orders.Where(o => o.ConsigneeName.ToLower().Contains(orderModel.SearchConsigneeName.ToLower())).ToList();
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

            if (orderModel.SortConsigneeName == "abc")
                orders = orders.OrderBy(o => o.ConsigneeName).ToList();
            else if (orderModel.SortConsigneeName == "zxy")
                orders = orders.OrderByDescending(o => o.ConsigneeName).ToList();

            return orders;
        }

        public int GetCountOrder(OrderModel orderCondition)
        {
            int countOrder = GetOrderDetailExcuteCondition(orderCondition).Count;

            return countOrder;
        }

        public void AddOrder(Models.Order order)
        {
            if(order.DiscountId == 0)
            {
                order.DiscountId = null;
            }
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public float GetTotalProductSale()
        {
            var totalAmount = (from o in _context.Orders
                               select o.TotalAmount).Sum();
            return totalAmount;
        }

        public OrderDetailViewModel? GetOrderDetailById(long orderId)
        {
            OrderDetailViewModel? order = _context.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => new OrderDetailViewModel
                {
                    CustomerId = o.CustomerId,
                    Email = o.Email,
                    FullName = o.FullName,
                    Phone = o.Phone,
                    ShipAddress = o.ShipAddress,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymetMethod,
                    OrderId = o.OrderId.ToString(),
                    ConsigneeName = o.ConsigneeFullName,
                    ConsigneePhone = o.ConsigneePhone,
                    totalOrderItems = _context.OrderItems.Count(ot => ot.OrderId == o.OrderId),
                    DiscountId = o.DiscountId,
                    Status = o.Status,
                    ShippingFee = o.ShippingFee,
                    ReturnId = o.ReturnId
                })
                .FirstOrDefault();

            return order;
        }

        public void UpdateStatusOrder(long orderId, string status, int shipper)
        {
            if (shipper != 0 && shipper != -1)
            {
                var order = _context.Orders.FirstOrDefault(order => order.OrderId == orderId);
                order.Status = status;
                order.ShipperId = shipper;
                _context.SaveChanges();
            }
            else if(shipper == -1)
            {
                var order = _context.Orders.FirstOrDefault(order => order.OrderId == orderId);
                order.Status = status;
                _context.SaveChanges();
            }else
            {
                var order = _context.Orders.FirstOrDefault(order => order.OrderId == orderId);
                order.Status = status;
                order.ShipperId = null;
                _context.SaveChanges();
                if(status == "Đã hủy")
                {
                    var orderItem = _context.OrderItems.Where(oi => oi.OrderId == orderId).ToList();
                    foreach (var item in orderItem)
                    {
                        var productOption = _context.ProductOptions.FirstOrDefault(po => po.ProductOptionId == item.ProductOptionId);
                        productOption.Quantity += item.Quantity;
                        _context.SaveChanges();
                    }
                }
            }
        }

        public void UpdateReturnOrder(long orderId, int returnId)
        {
            var order = _context.Orders.FirstOrDefault(order => order.OrderId == orderId);
            order.ReturnId = returnId;
            _context.SaveChanges();
        }
    }
}
