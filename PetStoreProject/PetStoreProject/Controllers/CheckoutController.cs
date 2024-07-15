using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Cart;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.Discount;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.OrderItem;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICustomerRepository _customer;
        private readonly IConfiguration _configuration;
        private readonly ICartRepository _cart;
        private readonly IOrderRepository _order;
        private readonly IOrderItemRepository _orderItem;
        private readonly IDiscountRepository _discount;
        private readonly PetStoreDBContext _context;

        public CheckoutController(ICustomerRepository customer, IConfiguration configuration,
            ICartRepository cart, IOrderRepository order, IOrderItemRepository orderItem, IDiscountRepository discount, PetStoreDBContext context)
        {
            _customer = customer;
            _configuration = configuration;
            _cart = cart;
            _order = order;
            _orderItem = orderItem;
            _discount = discount;
            _context = context;
        }

        [HttpPost]
        public IActionResult Form([FromBody] List<ItemsCheckoutViewModel> selectedProductCheckout)
        {
            if (selectedProductCheckout == null || !selectedProductCheckout.Any())
            {
                return BadRequest("No items to checkout");
            }

            List<int> itemCheckoutIds = new List<int>();
            foreach (var item in selectedProductCheckout)
            {
                item.Promotion = _cart.GetItemPromotion(item.ProductOptionId);
                itemCheckoutIds.Add(item.ProductOptionId);
            }

            // Lưu trữ dữ liệu vào session hoặc viewdata
            TempData["selectedProducts"] = Newtonsoft.Json.JsonConvert.SerializeObject(selectedProductCheckout);

            Response.Cookies.Append("Checkout_Id", Newtonsoft.Json.JsonConvert.SerializeObject(itemCheckoutIds));
            return Ok();
        }

        [HttpGet]
        public IActionResult Form()
        {
            // Lấy dữ liệu từ TempData
            var selectedProductsJson = TempData["selectedProducts"] as string;

            var selectedProductCheckout = string.IsNullOrEmpty(selectedProductsJson) ? null :
                Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemsCheckoutViewModel>>(selectedProductsJson);

            var email = HttpContext.Session.GetString("userEmail");
            if (email != null)
            {
                Customer customer = _customer.GetCustomer(email);
                if (customer != null)
                {
                    ViewBag.customer = customer;
                }
            }

            var total_amount = 0.0;
            if(selectedProductCheckout != null)
            {
                foreach (var item in selectedProductCheckout)
                {
                    var priceItem = 0.0;
                    if (item.Promotion != null && item.Promotion.Value != null)
                    {
                        priceItem = item.Price * (1 - (double)item.Promotion.Value / 100);
                    }
                    else
                    {
                        priceItem = item.Price;
                    }
                    total_amount += priceItem * item.Quantity;
                }

                var discounts = _discount.GetDiscounts(total_amount, email);
                ViewData["Discounts"] = discounts;
                return View(selectedProductCheckout);
            }else
            {
                return View(null);
            }
            
        }

        [HttpPost]
        public IActionResult ProcessCheckout([FromBody] CheckoutViewModel checkout)
        {
            var orderId = DateTime.Now.Ticks.ToString();
            var amount = checkout.TotalAmount;

            checkout.OrderId = long.Parse(orderId);
            Response.Cookies.Append("CheckoutInfo", Newtonsoft.Json.JsonConvert.SerializeObject(checkout));

            if(checkout.PaymentMethod.Equals("VNPay"))
            {
                return Json(new
                {
                    UrlTransfer = "CreatePayment",
                    OrderId = orderId,
                    Amount = amount,
                });
            }else
            {
                return Json(new
                {
                    UrlTransfer = "ProcessCashOnDelivery",
                    OrderId = orderId,
                    Amount = amount,
                });
            }

        }

        


        public IActionResult CreatePayment(string orderId, int amount)
        {
            var context = HttpContext;
            // Lấy thông tin cấu hình từ file config
            string vnp_ReturnUrl = _configuration["VNPay:ReturnUrl"];
            string vnp_Url = _configuration["VNPay:BaseUrl"];
            string vnp_TmnCode = _configuration["VNPay:TmnCode"];
            string vnp_HashSecret = _configuration["VNPay:HashSecret"];

            // Tạo mã đơn hàng duy nhất và xác định số tiền thanh toán
            var vnpay = new VnPayLibrary();

            // Thêm các thông tin cần thiết cho yêu cầu thanh toán
            vnpay.AddRequestData("vnp_Version", _configuration["VNPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _configuration["VNPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString());
            vnpay.AddRequestData("vnp_CurrCode", _configuration["VNPay:CurrCode"]);
            vnpay.AddRequestData("vnp_TxnRef", orderId);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_Locale", _configuration["VNPay:Locale"]);
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

            // Tạo URL thanh toán với các thông tin đã chuẩn bị
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            // Chuyển hướng người dùng đến URL thanh toán của VNPAY
            return Redirect(paymentUrl);
        }

        public VNPaymentResponseViewModel PaymentExcute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["VNPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VNPaymentResponseViewModel
                {
                    Success = false
                };
            }

            return new VNPaymentResponseViewModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }

        public IActionResult PaymentCallBack()
        {
            var response = PaymentExcute(Request.Query);

            var CheckoutCookie = Request.Cookies["Checkout_Id"];

            if (response == null || response.VnPayResponseCode != "00")
            {
                if (CheckoutCookie != null)
                {
                    Response.Cookies.Delete("Checkout_Id");
                }

                TempData["Status"] = "Thanh toán thất bại";
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("NotificationPayment");
            }

            // Lưu đơn hàng vào database
            var checkoutInfoCookie = Request.Cookies["CheckoutInfo"];
            CheckoutViewModel checkoutInfo = new CheckoutViewModel();
            if (checkoutInfoCookie != null)
            {
                checkoutInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckoutViewModel>(checkoutInfoCookie);
            }

            Order order = new Order()
            {
                OrderId = checkoutInfo.OrderId,
                FullName = checkoutInfo.OrderName,
                CustomerId = null,
                Email = null,
                Phone = checkoutInfo.OrderPhone,
                TotalAmount = checkoutInfo.TotalAmount,
                OrderDate = DateTime.Now,
                ConsigneeFullName = checkoutInfo.ConsigneeName,
                ConsigneePhone = checkoutInfo.ConsigneePhone,
                PaymetMethod = checkoutInfo.PaymentMethod,
                ShipAddress = checkoutInfo.ConsigneeProvince + ", " + checkoutInfo.ConsigneeDistrict + ", "
                                + checkoutInfo.ConsigneeWard + ", " + checkoutInfo.ConsigneeAddressDetail,
                DiscountId = checkoutInfo.DiscountId,
            };



            //Xóa phần tử trong database || cookie
            var email = HttpContext.Session.GetString("userEmail");
            var customerID = _customer.GetCustomerId(email);

            List<int> productOptionId = new List<int>();
            productOptionId = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(CheckoutCookie);
            if (email != null)
            {
                //Thêm order
                order.CustomerId = customerID;
                order.Email = email;

                _order.AddOrder(order);

                foreach (var item in checkoutInfo.OrderItems)
                {
                    OrderItem orderItem = new OrderItem()
                    {
                        OrderId = checkoutInfo.OrderId,
                        ProductOptionId = item.ProductOptionId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        PromotionId = item.PromotionId,
                    };
                    _orderItem.AddOrderItem(orderItem);
                }

                //Xóa cart
                var cartItems = _cart.GetListCartItemsVM(customerID);
                foreach (var item in cartItems)
                {
                    if (productOptionId.Contains(item.ProductOptionId))
                    {
                        _cart.DeleteCartItem(item.ProductOptionId, customerID);
                    }
                }
            }
            else
            {
                //Them order
                order.Email = checkoutInfo.OrderEmail;

                _order.AddOrder(order);

                foreach (var item in checkoutInfo.OrderItems)
                {
                    OrderItem orderItem = new OrderItem()
                    {
                        OrderId = checkoutInfo.OrderId,
                        ProductOptionId = item.ProductOptionId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        PromotionId = item.PromotionId,
                    };
                    _orderItem.AddOrderItem(orderItem);
                }

                //Xoa cart
                CookieOptions cookieOptions = new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(1), // Thời hạn tồn tại của cookie
                };

                if (Request.Cookies.TryGetValue("Items_id", out string list_cookie))
                {
                    var cookieIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(list_cookie);
                    List<int> productOptionIdRemaining = new List<int>();
                    foreach (var itemCookie in cookieIds)
                    {
                        if (productOptionId.Contains(itemCookie))
                        {
                            var item = Request.Cookies[$"Item_{itemCookie}"];
                            if (item != null) Response.Cookies.Delete($"Item_{itemCookie}");
                        }
                        else
                        {
                            productOptionIdRemaining.Add(itemCookie);
                        }
                    }
                    if (productOptionIdRemaining.Count > 0)
                    {
                        Response.Cookies.Append("Items_id", JsonConvert.SerializeObject(productOptionIdRemaining), cookieOptions);
                    }
                    else
                    {
                        Response.Cookies.Delete("Items_id");
                    }

                }
                
            }
            var discountId = checkoutInfo.DiscountId;
            if (discountId != null && discountId != 0)
            {
                var discount = _context.Discounts.Where(d => d.DiscountId == discountId).FirstOrDefault();
                discount.Used += 1;
                _context.SaveChanges();
            }
            Response.Cookies.Delete("Checkout_Id");
            Response.Cookies.Delete("CheckoutInfo");

            TempData["Status"] = "Thanh toán thành công";
            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("NotificationPayment");
        }

        public IActionResult NotificationPayment()
        {
            return View();
        }
    }
}
