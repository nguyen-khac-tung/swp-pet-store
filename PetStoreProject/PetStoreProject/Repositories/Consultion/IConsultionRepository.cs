using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Consultion
{
    public interface IConsultionRepository
    {
        public List<ConsultionViewForAdmin> GetListConsultion();
        int CreateConsultion(ConsultionCreateRequestViewModel consultion);
        public ConsultionViewForAdmin GetDetail(int id);
        public void Reply(int id, string message);
    }
}
