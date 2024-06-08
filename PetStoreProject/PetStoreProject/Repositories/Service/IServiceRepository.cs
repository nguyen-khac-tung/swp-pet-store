using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Service
{
    public interface IServiceRepository
    {
        public List<ServiceViewModel> GetListServices();
    }
}
