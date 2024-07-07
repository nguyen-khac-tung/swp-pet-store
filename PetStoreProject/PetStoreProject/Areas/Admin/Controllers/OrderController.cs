using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Order;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {

        private readonly IOrderRepository _order;

        public OrderController(IOrderRepository order)
        {
            _order = order;
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

            return Json( new {
                OrderHistory = listOrderHistory,
                PageIndex = orderCondition.pageIndex,
                TotalOrder = totalOrder,
                NumberPage = totalPage,
            });
        }

    }
}

