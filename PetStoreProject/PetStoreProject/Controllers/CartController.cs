using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetStoreProject.Repositories.Cart;
using PetStoreProject.Repositories.Product;
using PetStoreProject.ViewModels;

namespace PetStoreProject.CartController
{
	[Route("[controller]/[action]")]
	public class CartController : Controller
	{
		private readonly IProductRepository _product;
		private readonly ICartRepository _cart;

		public CartController(IProductRepository product, ICartRepository cart)
		{
			_product = product;
			_cart = cart;
		}

		[HttpPost]
		public ActionResult AddItem(int productOptionId, int quantity)
		{
			List<int> cookiesId = new List<int>();
			bool isExistsItem = false;

			var new_item = _cart.GetCartItemVM(productOptionId, quantity);

			var cookieOptions = new CookieOptions
			{
				Expires = DateTime.Now.AddDays(1), // Thời hạn tồn tại của cookie
				HttpOnly = true, // Cookie chỉ được sử dụng trong HTTP(S) requests
				Secure = true, // Cookie chỉ được gửi qua HTTPS
				SameSite = SameSiteMode.Strict // Chỉ gửi cookie trong cùng site
			};

			if (Request.Cookies.TryGetValue("Items_id", out string list_cookie))
			{
				cookiesId = JsonConvert.DeserializeObject<List<int>>(list_cookie);
				foreach (var itemId in cookiesId)
				{
					if (itemId == productOptionId)
					{
						isExistsItem = true;
						if (Request.Cookies.TryGetValue($"Item_{itemId}", out string cookieItem))
						{
							var cartItem = JsonConvert.DeserializeObject<CartItemViewModel>(cookieItem);
							if (!(cartItem.Quantity + quantity > 10))
							{
								cartItem.Quantity += quantity;
								Response.Cookies.Append($"Item_{itemId}", JsonConvert.SerializeObject(cartItem), cookieOptions);
								return Json(new
								{
									message = "success"
								});
							}
							else
							{
								return Json(new
								{
									message = "Khoong thể mua quá 10 sản phẩm cho 1 món hàng!!! Vui lòng thanh toán để có thể mua thêm"
								});
							}
						}
					}
				}
			}
			if (!isExistsItem)
			{
				cookiesId.Add(productOptionId);
				Response.Cookies.Append($"Items_id", JsonConvert.SerializeObject(cookiesId), cookieOptions);
				Response.Cookies.Append($"Item_{productOptionId}", JsonConvert.SerializeObject(new_item), cookieOptions);
			}

			return Json(new
			{
				message = "success"
			});
		}

		[HttpPost]
		public ActionResult GetCartItems()
		{
			List<int> cookiesId = new List<int>();
			List<CartItemViewModel> cartItems = new List<CartItemViewModel>();

			if (Request.Cookies.TryGetValue("Items_id", out string list_cookie))
			{
				cookiesId = JsonConvert.DeserializeObject<List<int>>(list_cookie);
				foreach (var itemId in cookiesId)
				{
					if (Request.Cookies.TryGetValue($"Item_{itemId}", out string cookieItem))
					{
						var item = JsonConvert.DeserializeObject<CartItemViewModel>(cookieItem);
						cartItems.Add(item);
					}
				}
			}
			return Json(cartItems);
		}


		[HttpGet]
		public ActionResult Detail()
		{
			List<int> cookiesId = new List<int>();
			List<CartItemViewModel> cartItems = new List<CartItemViewModel>();
			if (Request.Cookies.TryGetValue("Items_id", out string list_cookie))
			{
				cookiesId = JsonConvert.DeserializeObject<List<int>>(list_cookie);
				foreach (var itemId in cookiesId)
				{
					if (Request.Cookies.TryGetValue($"Item_{itemId}", out string cookieItem))
					{
						var item = JsonConvert.DeserializeObject<CartItemViewModel>(cookieItem);
						cartItems.Add(item);
					}
				}
			}
			ViewData["cartItems"] = cartItems;
			return View();
		}
	}


}
