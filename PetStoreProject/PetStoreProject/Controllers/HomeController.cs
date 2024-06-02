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

		public List<ProductDetailViewModel> GetListItemToDisplayed(List<ProductDetailViewModel> Items)
		{
			List<ProductDetailViewModel> itemsDisplayed = new List<ProductDetailViewModel>();
			foreach (var item in Items)
			{
				bool isSoldOut = false;
				foreach (var option in item.productOption)
				{
					if (option.IsSoldOut == true)
					{
						isSoldOut = true;
					}
				}

				if (isSoldOut == false)
				{
					itemsDisplayed.Add(item);
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

			var dogFoods = _product.GetProductDetailDoGet(new List<int> { 3 }, 14);
			var catFoods = _product.GetProductDetailDoGet(new List<int> { 4 }, 20);
			var dogAccessories = _product.GetProductDetailDoGet(new List<int> { 2, 6 }, 0);
			var catAccessories = _product.GetProductDetailDoGet(new List<int> { 6 }, 0);
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
