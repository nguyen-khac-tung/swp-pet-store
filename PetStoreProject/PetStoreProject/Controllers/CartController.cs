using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Cart;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.ViewModels;

namespace PetStoreProject.CartController
{
    public class CartController : Controller
    {
        private readonly ICartRepository _cart;
        private readonly ICustomerRepository _customer;
        private readonly IAccountRepository _account;

        public CartController(ICartRepository cart, ICustomerRepository customer, IAccountRepository account)
        {
            _cart = cart;
            _customer = customer;
            _account = account;
        }

        public string CheckUserRole()
        {
            var email = HttpContext.Session.GetString("userEmail");
            if (email != null)
            {
                var role = _account.GetUserRoles(email);
                if (role == "Customer")
                {
                    return "Customer";
                }
                else
                {
                    return "Not Guest or Customer";
                }
            }
            else
            {
                return "Guest";
            }
        }

        [HttpPost]
        public ActionResult GetCartBoxItems()
        {
            var userRole = CheckUserRole();
            if (userRole == "Customer")
            {
                var customerEmail = HttpContext.Session.GetString("userEmail");
                var customerID = _customer.GetCustomerId(customerEmail);
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
            var userRole = CheckUserRole();
            if (userRole == "Customer")
            {
                var customerEmail = HttpContext.Session.GetString("userEmail");
                var customerID = _customer.GetCustomerId(customerEmail);
                return AddCartItemOfCustomer(productOptionId, quantity, customerID);
            }
            else if (userRole == "Guest")
            {
                return AddCartItemOfGuest(productOptionId, quantity);
            }
            else
            {
                return Json(new { message = "Tài khoản của bạn không thể sử dụng chức năng này!!!" });
            }
        }

        [HttpGet]
        public ActionResult Detail()
        {
            var userRole = CheckUserRole();
            if (userRole == "Customer")
            {
                var customerEmail = HttpContext.Session.GetString("userEmail");
                var customerID = _customer.GetCustomerId(customerEmail);
                return CartDetailOfCustomer(customerID);
            }
            else if (userRole == "Guest")
            {
                return CartDetailOfGuest();
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account", new { allowedRoles = new string[] { "Customer" } });
            }
        }

        [HttpDelete]
        public ActionResult Delete(int productOptionId)
        {
            var userRole = CheckUserRole();
            if (userRole == "Customer")
            {
                var customerEmail = HttpContext.Session.GetString("userEmail");
                var customerID = _customer.GetCustomerId(customerEmail);
                return DeleteCartOfCustomer(productOptionId, customerID);
            }
            else if (userRole == "Guest")
            {
                return DeleteCartOfGuest(productOptionId);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account", new { allowedRoles = new string[] { "Customer" } });
            }
        }

        [HttpPut]
        public ActionResult Edit(int oldProductOptionId, int newProductOptionId, int quantity)
        {
            var userRole = CheckUserRole();
            if (userRole == "Customer")
            {
                var customerEmail = HttpContext.Session.GetString("userEmail");
                var customerID = _customer.GetCustomerId(customerEmail);
                return EditCartOfCustomer(oldProductOptionId, newProductOptionId, quantity, customerID);
            }
            else if (userRole == "Guest")
            {
                return EditCartOfGuest(oldProductOptionId, newProductOptionId, quantity);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account", new { allowedRoles = new string[] { "Customer" } });
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
                        var check = _cart.GetCartItemVM(item.ProductOptionId, item.Quantity);
                        if (check.isDeleted == false)
                        {
                            cartItems.Add(item);
                        }
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
                totalPrice += cartItem.Price * cartItem.Quantity;
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

        public ActionResult DeleteCartOfCustomer(int productOptionId, int customerID)
        {
            _cart.DeleteCartItem(productOptionId, customerID);
            return Json(new { message = "success" });
        }

        public ActionResult DeleteCartOfGuest(int productOptionId)
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

        public ActionResult EditCartOfCustomer(int oldProductOptionId, int newProductOptionId, int quantity, int customerID)
        {
            if (oldProductOptionId == newProductOptionId)
            {
                _cart.UpdateNewCartItem(oldProductOptionId, newProductOptionId, quantity, customerID);
                var newItem = _cart.findCartItemViewModel(oldProductOptionId, customerID);
                return Json(newItem);
            }
            else
            {
                bool isExist = _cart.isExistProductOption(newProductOptionId, customerID);
                if (isExist)
                {
                    return Json(new { message = "Sản phẩm đã tồn tại trong giỏ hàng!!!" });
                }
                else
                {
                    _cart.UpdateNewCartItem(oldProductOptionId, newProductOptionId, quantity, customerID);
                    var cartItem = _cart.findCartItemViewModel(newProductOptionId, customerID);
                    return Json(cartItem);
                }
            }
        }

        public ActionResult EditCartOfGuest(int oldProductOptionId, int newProductOptionId, int quantity)
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
