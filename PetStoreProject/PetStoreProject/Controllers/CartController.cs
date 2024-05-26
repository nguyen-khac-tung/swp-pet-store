using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Cart;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.Product;
using PetStoreProject.ViewModels;

namespace PetStoreProject.CartController
{
    [Route("[controller]/[action]")]
    public class CartController : Controller
    {
        private readonly IProductRepository _product;
        private readonly ICartRepository _cart;
        private readonly ICustomerRepository _customer;

        public CartController(IProductRepository product, ICartRepository cart, ICustomerRepository customer)
        {
            _product = product;
            _cart = cart;
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

        [HttpPost]
        public ActionResult GetCartBoxItems()
        {
            var customerID = getCustomerId();
            if (customerID != -1)
            {
                return GetCartBoxItemsOfCustomer(customerID);
            }
            else
            {
                return GetCartBoxItemsOfGuest();
            }
        }

        [HttpPost]
        public ActionResult AddToCart(int productOptionId, int quantity)
        {
            var customerID = getCustomerId();
            if (customerID != -1)
            {
                return AddCartItemOfCustomer(productOptionId, quantity, customerID);
            }
            else
            {
                return AddCartItemOfGuest(productOptionId, quantity);
            }
        }

        [HttpGet]
        public ActionResult Detail()
        {
            var customerID = getCustomerId();
            if (customerID != -1)
            {
                return CartDetailOfCustomer(customerID);
            }
            else
            {
                return CartDetailOfGuest();
            }
        }

        public ActionResult GetCartBoxItemsOfCustomer(int customerID)
        {
            List<CartItemViewModel> cartItems = _cart.GetListCartItemsVM(customerID);
            return Json(cartItems);
        }

        public ActionResult GetCartBoxItemsOfGuest()
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

        public ActionResult AddCartItemOfCustomer(int productOptionId, int quantity, int customerID)
        {
            var cartItems = _cart.GetListCartItemsVM(customerID);
            var cartItem = (from item in cartItems
                            where item.ProductOptionId == productOptionId
                            select item).FirstOrDefault();
            if (cartItem != null)
            {
                if (cartItem.Quantity + quantity > 10)
                {
                    return Json(new { message = "Không thể mua quá 10 sản phẩm cho 1 món hàng!!! Vui lòng thanh toán để có thể mua thêm" });
                }
                else
                {
                    _cart.UpdateQuantityToCartItem(productOptionId, quantity, customerID);
                    return Json(new { message = "success" });
                }
            }
            else
            {
                _cart.AddItemsToCart(productOptionId, quantity, customerID);
                return Json(new { message = "success" });
            }
        }

        public ActionResult AddCartItemOfGuest(int productOptionId, int quantity)
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
                                    message = "Không thể mua quá 10 sản phẩm cho 1 món hàng!!! Vui lòng thanh toán để có thể mua thêm"
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

        public ActionResult CartDetailOfCustomer(int customerID)
        {
            var cartItems = _cart.GetListCartItemsVM(customerID);
            float totalPrice = 0;
            foreach (var cartItem in cartItems)
            {
                totalPrice += cartItem.Price* cartItem.Quantity;
            }
            ViewData["cartItems"] = cartItems;
            ViewData["total_price"] = totalPrice;
            return View("~/Views/Cart/Detail.cshtml");
        }

        public ActionResult CartDetailOfGuest()
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
            var total_price = 0.0;
            foreach (var item in cartItems)
            {
                total_price += item.Price * item.Quantity;
            }
            ViewData["cartItems"] = cartItems;
            ViewData["total_price"] = total_price;
            return View("~/Views/Cart/Detail.cshtml");
        }

        [HttpDelete]
        public ActionResult Delete(int productOptionId)
        {
            var cookieOptionDelete = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            if (Request.Cookies.TryGetValue("Items_id", out string list_cookie))
            {
                List<int> listId = JsonConvert.DeserializeObject<List<int>>(list_cookie);
                listId.Remove(productOptionId);
                Response.Cookies.Append("Items_id", JsonConvert.SerializeObject(listId));
            }

            if (Request.Cookies.TryGetValue($"Item_{productOptionId}", out string item))
            {
                Response.Cookies.Append($"Item_{productOptionId}", item, cookieOptionDelete);
                return Json(new
                {
                    message = "success"
                });
            }
            return Json(new
            {
                message = "Error"
            });
        }

        [HttpPut]
        public ActionResult Edit(int oldProductOptionId, int newProductOptionId, int quantity)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1), // Thời hạn tồn tại của cookie
                HttpOnly = true, // Cookie chỉ được sử dụng trong HTTP(S) requests
                Secure = true, // Cookie chỉ được gửi qua HTTPS
                SameSite = SameSiteMode.Strict // Chỉ gửi cookie trong cùng site
            };

            var cookieOptionDelete = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            };

            var new_item = _cart.GetCartItemVM(newProductOptionId, quantity);
            if (oldProductOptionId == newProductOptionId)
            {
                Response.Cookies.Append($"Item_{newProductOptionId}", JsonConvert.SerializeObject(new_item), cookieOptions);
                return Json(new_item);
            }

            List<int> cookiesId = new List<int>();
            bool isExistsItem = false;

            if (Request.Cookies.TryGetValue("Items_id", out string list_cookie))
            {
                cookiesId = JsonConvert.DeserializeObject<List<int>>(list_cookie);
                foreach (var id in cookiesId)
                {
                    if (id != oldProductOptionId && id == newProductOptionId)
                    {
                        return Json(new
                        {
                            message = "Sản phẩm đã tồn tại trong giỏ hàng!!!"
                        });
                    }
                }
            }

            cookiesId.Add(newProductOptionId);
            cookiesId.Remove(oldProductOptionId);
            Response.Cookies.Append("Items_id", JsonConvert.SerializeObject(cookiesId), cookieOptions);
            Response.Cookies.Append($"Item_{newProductOptionId}", JsonConvert.SerializeObject(new_item), cookieOptions);
            Response.Cookies.Append($"Item_{oldProductOptionId}", "old", cookieOptionDelete);
            return Json(new_item);
        }
    }
}
