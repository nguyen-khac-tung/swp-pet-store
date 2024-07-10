using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Models;

namespace PetStoreProject.Areas.Employee.Controllers
{
	[Area("Employee")]
	public class NewsController : Controller
	{
		private readonly PetStoreDBContext _dbContext;
		private readonly ICloudinaryService _cloudinaryService;

		public NewsController(PetStoreDBContext petStoreDBContext, ICloudinaryService cloudinaryService)
		{
			_dbContext = petStoreDBContext;
			_cloudinaryService = cloudinaryService;
		}
		public IActionResult CreateNews()
		{
			var listag = _dbContext.TagNews.ToList();
			return View(listag);
		}
		[HttpPost]
		public async Task<IActionResult> SaveContent(string content, string title, string summary, IFormFile thumbnail)
		{
			var email = HttpContext.Session.GetString("userEmail");
			var eid = _dbContext.Employees.Where(e => e.Email == email).Select(e => e.EmployeeId).FirstOrDefault();
			var article = new News { Content = content, Title = title, Summary = summary, EmployeeId = eid, DatePosted = DateOnly.FromDateTime(DateTime.Now) };
			var uploadResult = await _cloudinaryService.UploadImage(thumbnail);
			var url = uploadResult.Url.ToString();
			int maxImgId = _dbContext.Images.Max(img => img.ImageId);
			var newImage = new Image
			{
				ImageId = ++maxImgId,
				ImageUrl = url!,
				News = article,
			};
			_dbContext.News.Add(article);
			_dbContext.Images.Add(newImage);
			await _dbContext.SaveChangesAsync();
			return RedirectToAction("CreateNews");
		}
		[HttpPost]
		public async Task<IActionResult> UploadImage(IFormFile file)
		{
			var uploadResult = await _cloudinaryService.UploadImage(file);

			if (uploadResult == null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return BadRequest("Could not upload image to Cloudinary");
			}

			var url = uploadResult.Url.ToString();
			return Json(new { location = url });
		}
	}
}
