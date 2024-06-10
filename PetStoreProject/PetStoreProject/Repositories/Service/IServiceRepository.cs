using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Service
{
    public interface IServiceRepository
    {
        public List<ServiceViewModel> GetListServices();

        public ServiceDetailViewModel GetServiceDetail(int serviceId);

        public ServiceOptionViewModel GetFistServiceOption(int serviceId);

        public ServiceOptionViewModel GetFirstServiceAndListWeightOfPetType(int serviceId, string petType);
        
        public ServiceOptionViewModel GetNewServiceOptionBySelectWeight(int serviceId, string petType, string weight);

        public BookServiceViewModel GetBookingServiceInFo(int serviceOptionId);

        public List<TimeOnly> GetWorkingTime(int serviceId);

        public void SaveBookServiceForm(BookServiceViewModel bookServiceInfo);

        public List<ServiceViewModel> GetOtherServices(int serviceId);

    }
}
