using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly PetStoreDBContext _context;

        public FeedbackController(PetStoreDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Submit()
        {
            int pid = Int32.Parse(Request.Form["pid"]);
			Feedback feedback = new Feedback();
            feedback.Name = Request.Form["FullName"];
            feedback.Phone = Request.Form["PhoneNumber"];
            feedback.Email = Request.Form["Email"];
            feedback.Rating = Int32.Parse(Request.Form["rating"]);
            feedback.Content = Request.Form["Content"];
            feedback.DateCreated = DateTime.Now;
            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();
			return RedirectToAction("Detail", "Product", new { productId = Request.Form["pid"] });
		}

        [HttpPost]
        public JsonResult CheckPhoneNumber(string phoneNumber)
        {
            bool phoneNumberExistsOrder = _context.Orders.Any(o => o.Phone == phoneNumber);

            if (!phoneNumberExistsOrder)
            {
                return Json(new { status = "error", message = "Bạn không thể comment vì bạn chưa mua hàng." });
            }

            return Json(new { status = "success"});
        }
    }
}
