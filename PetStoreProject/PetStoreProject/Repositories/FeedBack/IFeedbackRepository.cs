using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.FeedBack
{
    public interface IFeedbackRepository
    {
        public List<FeedBackViewModels> GetListFeedBack(int page);

        public List<FeedbackViewModels> GetListFeedBackForService(int serviceId);
    }
}
