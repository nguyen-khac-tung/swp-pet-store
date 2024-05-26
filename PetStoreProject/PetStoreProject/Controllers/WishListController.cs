using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.WishList;

namespace PetStoreProject.Controllers
{
	public class WishListController : Controller
	{
		private readonly IWishListRepository _wishList;

		public WishListController(IWishListRepository wishList)
		{
			_wishList = wishList;
		}
		public IActionResult Detail(int customerID)
		{
			var listWishList = _wishList.wishListVMs(22);
			return View(listWishList);
		}
		[HttpPost]
		public IActionResult Delete(int productID)
		{
			_wishList.DeleteFromWishList(22, productID);
			return RedirectToAction("Detail");
		}
	}
}
