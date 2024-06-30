using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICustomerRepository _customer;
        private readonly IAccountRepository _account;

        public CheckoutController(ICustomerRepository customer, IAccountRepository account)
        {
            _customer = customer;
            _account = account;
        }

        private string CheckUserRole()
        {
            var email = HttpContext.Session.GetString("userEmail");
            var role = _account.GetUserRoles(email);

            if (email != null)
            {
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
        public IActionResult Form([FromBody] List<ItemsCheckoutViewModel> selectedProductCheckout)
        {
            if (selectedProductCheckout == null || !selectedProductCheckout.Any())
            {
                return BadRequest("No items to checkout");
            }

            // Lưu trữ dữ liệu vào session hoặc viewdata
            TempData["selectedProducts"] = Newtonsoft.Json.JsonConvert.SerializeObject(selectedProductCheckout);

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

            return View(selectedProductCheckout);
        }

        [HttpPost]
        public IActionResult ProcessCheckout([FromBody] CheckoutViewModel checkout)
        {

            var orderId = DateTime.Now.Ticks.ToString();
            var amount = checkout.TotalAmount;
            return RedirectToAction("CreatePayment", new { orderId = orderId, amount = amount });
        }

        public IActionResult CreatePayment(string OrderId, int amount)
        {


            return View();
        }
    }
}
