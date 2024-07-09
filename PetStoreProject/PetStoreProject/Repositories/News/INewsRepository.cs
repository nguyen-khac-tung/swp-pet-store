using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.News
{
    public interface INewsRepository
    {
        public NewsViewModel GetNewsById(int id);

        public List<NewsViewModel> GetListNews();
    }
}
