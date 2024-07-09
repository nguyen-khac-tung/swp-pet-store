using Microsoft.EntityFrameworkCore.Migrations;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.News
{
    public class NewsRepository : INewsRepository
    {
        private readonly PetStoreDBContext _dbContext;

        public NewsRepository(PetStoreDBContext petStoreDBContext)
        {
            _dbContext = petStoreDBContext;
        }

        public List<NewsViewModel> GetListNews()
        {
            var listNews = (from img in _dbContext.Images
                            join n in _dbContext.News on img.ImageId equals n.NewsId
                            where n.IsDelete == false
                            select new NewsViewModel
                            {
                                url_thumnail = img.ImageUrl,
                                NewsId = n.NewsId,
                                Title = n.Title,
                                Description = n.Summary,
                                DateOnly = n.DatePosted,
                            }).OrderByDescending(n => n.DateOnly).ToList();
            return listNews;
        }

        public NewsViewModel GetNewsById(int id)
        {
            var news = (from img in _dbContext.Images
                        join n in _dbContext.News on img.ImageId equals n.NewsId
                        where n.NewsId == id && n.IsDelete == false   
                        select new NewsViewModel
                        {
                            url_thumnail = img.ImageUrl,
                            NewsId = id,
                            Title = n.Title,
                            Content = n.Content,
                            DateOnly = n.DatePosted,
                        }).FirstOrDefault();
            return news;
        }
    }
}
