using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.WishList;

namespace PetStoreProject.Controllers
{
	public class WishListController : Controller
	{
        private readonly ICustomerRepository _customer;
        private readonly IWishListRepository _wishList;

		public WishListController(ICustomerRepository customer, IWishListRepository wishList)
		{
            _customer = customer;
			_wishList = wishList;
		}

        public int getCustomerId()
        {
            var email = HttpContext.Session.GetString("Account");
            if (email != null)
            {
                var customerID = _customer.getCustomerId(email);
                return customerID;
            }
            else
            {
                return -1;
            }
        }

        public IActionResult Detail()
		{
			var customerId = getCustomerId();
			if(customerId != -1)
			{
                var listWishList = _wishList.wishListVMs(customerId);
                return View(listWishList);
            }
            else
            {
                TempData["WishList_ErrorMessage"] = "Bạn cần phải đăng nhập để xem chi tiết.";
                return RedirectToAction("Index", "Home");
            }
			
		}
		[HttpPost]
		public IActionResult Delete(int productID)
		{
            var customerId = getCustomerId();
			_wishList.DeleteFromWishList(customerId, productID);
			return RedirectToAction("Detail");
		}
	}
}
