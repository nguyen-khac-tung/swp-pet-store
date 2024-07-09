using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.News;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Controllers
{
    public class NewsController : Controller
    {
        private readonly PetStoreDBContext _dbContext;
        private readonly INewsRepository _newsRepository;

        public NewsController(PetStoreDBContext petStoreDBContext, INewsRepository newsRepository)
        {
            _dbContext = petStoreDBContext;
            _newsRepository = newsRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

		public IActionResult Detail(int newsId)
		{
			NewsViewModel model = _newsRepository.GetNewsById(newsId);
			return View(model);
		}

		public IActionResult List()
        {
            var listNews = _newsRepository.GetListNews();
            return View(listNews);
        }
    }
}
