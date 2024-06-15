using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.FeedBack;

namespace PetStoreProject.Areas.Employee.Controllers
{
	[Area("Employee")]
	public class FeedbackController : Controller
	{
		private readonly IFeedbackRepository _feedback;	
		private readonly PetStoreDBContext _dbContext;
		public FeedbackController(IFeedbackRepository feedbackRepository, PetStoreDBContext petStoreDBContext)
		{
			_feedback = feedbackRepository;
			_dbContext = petStoreDBContext;
		}
		public IActionResult List()
		{
			var list = _feedback.GetListFeedBack();
			return View(list);
		}

        [HttpPost]
        public IActionResult Submit()
		{
            int fbId = Int32.Parse(Request.Form["FeedBackId"]);

            var feedBack = _dbContext.Feedbacks.SingleOrDefault(fb => fb.FeedbackId == fbId);

            if (feedBack != null)
                feedBack.Status = true;
            var email = HttpContext.Session.GetString("userEmail");
            // check have employee responed
            if (_dbContext.ResponseFeedbacks.Any(x => x.FeedbackId == fbId))
			{
				var respFb = _dbContext.ResponseFeedbacks.Where(x => x.FeedbackId ==fbId).FirstOrDefault();
				if(respFb != null)
				{
					respFb.EmployeeId = _dbContext.Employees.Where(e => e.Email == email).Select(e => e.EmployeeId).FirstOrDefault(); 
					respFb.Content = Request.Form["response"];
					respFb.DateCreated = DateTime.Now;
					_dbContext.SaveChanges();
                }
			}
			else
			{
                ResponseFeedback responseFeedback = new ResponseFeedback();
                responseFeedback.FeedbackId = fbId;
                responseFeedback.Content = Request.Form["response"];
                responseFeedback.DateCreated = DateTime.Now;
                responseFeedback.EmployeeId = _dbContext.Employees.Where(e => e.Email == email).Select(e => e.EmployeeId).FirstOrDefault();
                _dbContext.ResponseFeedbacks.Add(responseFeedback);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}
