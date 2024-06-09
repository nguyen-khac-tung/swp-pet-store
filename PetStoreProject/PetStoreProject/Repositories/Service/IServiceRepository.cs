using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Service
{
    public interface IServiceRepository
    {
        public List<ServiceViewModel> GetListServices();

        public ServiceDetailViewModel GetServiceDetail(int serviceId);

        public ServiceOptionViewModel GetFistServiceOption(int serviceId);

        public ServiceOptionViewModel GetFirstServiceAndListWeightOfPetType(int serviceId, string petType);

        public List<ServiceViewModel> GetOtherServices(int serviceId);

        public ServiceOptionViewModel GetNewServiceOptionBySelectWeight(int serviceId, string petType, string weight);
    }
}
