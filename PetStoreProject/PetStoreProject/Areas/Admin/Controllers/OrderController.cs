using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Discount;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.OrderItem;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {

        private readonly IOrderRepository _order;
        private readonly IDiscountRepository _discount;
        private readonly IOrderItemRepository _orderItem;

        public OrderController(IOrderRepository order, IDiscountRepository discount, IOrderItemRepository orderItem)
        {
            _order = order;
            _discount = discount;
            _orderItem = orderItem;
        }

        [HttpGet]
        public IActionResult ListOrderHistory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ListOrderHistory(OrderModel orderCondition)
        {
            var listOrderHistory = _order.GetOrderDetailByCondition(orderCondition);

            var totalOrder = _order.GetCountOrder(0);

            var totalPage = (int)Math.Ceiling((double)totalOrder / (double)10);

            var searchDate = orderCondition.SearchDate;
            ViewBag.searchDate = searchDate;

            return Json(new
            {
                OrderHistory = listOrderHistory,
                PageIndex = orderCondition.pageIndex,
                TotalOrder = totalOrder,
                NumberPage = totalPage,
            });
        }

        [HttpPost]
        public IActionResult Detail(OrderDetailViewModel order)
        {
            if(order.DiscountId.HasValue)
            {
                var discount = _discount.GetDiscount(order.DiscountId.Value);
                ViewBag.discount = discount;
            }else
            {
                ViewBag.discount = null;
            }
            var checkoutDetail = new CheckoutViewModel
            {
                OrderId = long.Parse(order.OrderId),
                OrderEmail = order.Email,
                OrderName = order.FullName,
                OrderPhone = order.Phone,
                ConsigneeAddressDetail = order.ShipAddress,
                ConsigneeName = order.ConsigneeName,
                ConsigneePhone = order.ConsigneePhone,
                PaymentMethod = order.PaymentMethod,
                TotalAmount = order.TotalAmount,
                ConsigneeWard = "",
                ConsigneeProvince = "",
                ConsigneeDistrict = "",
                OrderDate = order.OrderDate
            };

            var listItemOrder = _orderItem.GetOrderItemByOrderId(long.Parse(order.OrderId));

            ViewBag.listItemOrder = listItemOrder;

            return View(checkoutDetail);
        }
    }

}

