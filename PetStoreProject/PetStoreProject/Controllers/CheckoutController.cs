using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.ViewModels;

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

            return View(selectedProductCheckout);
        }
    }
}
