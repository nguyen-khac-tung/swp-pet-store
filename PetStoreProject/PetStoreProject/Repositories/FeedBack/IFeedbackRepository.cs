using PetStoreProject.Areas.Employee.ViewModels;

namespace PetStoreProject.Repositories.FeedBack
{
    public interface IFeedbackRepository
    {
        public List<FeedBackViewModels> GetListFeedBack();
    }
}
