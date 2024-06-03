using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Product;
using PetStoreProject.ViewModels;
using PetStoreProject.Repositories.Customers;

namespace PetStoreProject.Controllers
{
	public class HomeController : Controller
	{
		private readonly PetStoreDBContext _context;
		private readonly IProductRepository _product;
		private readonly ICustomerRepository _customer;

		public HomeController(PetStoreDBContext dbContext, IProductRepository product, ICustomerRepository customer)
		{
			_context = dbContext;
			_product = product;
			_customer = customer;
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

        public List<HomeProductViewModel> GetListItemToDisplayed(List<HomeProductViewModel> Items)
        {
            List<HomeProductViewModel> itemsDisplayed = new List<HomeProductViewModel>();
            foreach (var item in Items)
            {
                var product = _product.GetProductInStock(item);
                if (product != null)
                {
                    itemsDisplayed.Add(product);
                    if (itemsDisplayed.Count == 8)
                    {
                        break;
                    }
                }
            }
            return itemsDisplayed;
        }

        public IActionResult Index(string? success)
        {
            if (success != null)
            {
                ViewBag.Success = success;
            }

            List<int> listPID = _product.GetProductIDInWishList(getCustomerId());
            ViewData["listPID"] = listPID;

            var dogFoods = _product.GetProductsOfHome(3, 14);
            var catFoods = _product.GetProductsOfHome(4, 20);
            var dogAccessories = _product.GetProductsOfHome(2, null);
            var catAccessories = _product.GetProductsOfHome(6, null);
            var DogFoodsDisplayed = GetListItemToDisplayed(dogFoods);
            var homeVM = new HomeViewModel
            {
                NumberOfDogFoods = _product.GetNumberOfDogFoods(),
                NumberOfDogAccessories = _product.GetNumberOfDogAccessories(),
                NumberOfCatFoods = _product.GetNumberOfCatFoods(),
                NumberOfCatAccessories = _product.GetNumberOfCatAccessories(),
                DogFoodsDisplayed = GetListItemToDisplayed(dogFoods),
                CatFoodsDisplayed = GetListItemToDisplayed(catFoods),
                DogAccessoriesDisplayed = GetListItemToDisplayed(dogAccessories),
                CatAccessoriesDisplayed = GetListItemToDisplayed(catAccessories),
            };
            return View(homeVM);
        }
    }
}
