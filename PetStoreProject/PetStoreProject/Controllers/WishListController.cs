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

        public IActionResult Detail()
        {
            var listWishList = _wishList.wishListVMs(15);
            return View(listWishList);
        }
    }
}
