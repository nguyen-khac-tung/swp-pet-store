using Humanizer;
using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.Repositories.FeedBack;
using PetStoreProject.ViewModels;

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


        [HttpPost]
        public IActionResult LoadMoreFeedback(int page, int status, DateTime from, DateTime to)
        {
            var list = _feedback.GetListFeedBack(page).Where(x => x.CreatedDate > from && x.CreatedDate < to);
            if (status == 1)
            {
                list = list.Where(x => x.Status == false);
            }
            else if (status == 2)
            {
                list = list.Where(x => x.Status == true);
            }
            return new JsonResult(new
            {
                listResult = list
            });
        }

        [HttpPost]
        public IActionResult List(int status, DateTime from, DateTime to)
        {
            var list = _feedback.GetListFeedBack(1).Where(x => x.CreatedDate > from && x.CreatedDate < to);
            if (status == 1)
            {
                list = list.Where(x => x.Status == false);
            }
            else if (status == 2)
            {
                list = list.Where(x => x.Status == true);
            }

            ViewBag.Status = status;
            ViewBag.FirstDayOfWeek = from.ToString("yyyy-MM-dd");
            ViewBag.LastDayOfWeek = to.ToString("yyyy-MM-dd");
            ViewBag.Length = list.Count();
            return View(list);
        }


        public IActionResult List()
		{
            DateTime today = DateTime.Today;
            DateTime firstDayOfWeek = DateTimeHelpers.GetFirstDayOfWeek(today);
            DateTime lastDayOfWeek = DateTimeHelpers.GetLastDayOfWeek(today);
            var list = _feedback.GetListFeedBack(1).Where(x => x.CreatedDate > firstDayOfWeek && x.CreatedDate < lastDayOfWeek);
            ViewBag.FirstDayOfWeek = firstDayOfWeek.ToString("yyyy-MM-dd");
            ViewBag.LastDayOfWeek = lastDayOfWeek.ToString("yyyy-MM-dd");
            ViewBag.Status = 0;
            ViewBag.Length = list.Count();
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
