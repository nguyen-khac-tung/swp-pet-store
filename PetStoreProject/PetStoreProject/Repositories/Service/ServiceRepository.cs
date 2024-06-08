using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Service
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly PetStoreDBContext _context;

        public ServiceRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public List<ServiceViewModel> GetListServices()
        {
            var services = (from s in _context.Services
                            where s.IsDelete == false
                            select new ServiceViewModel
                            {
                                ServiceId = s.ServiceId,
                                Name = s.Name,
                                Type = s.Type,
                                SupDescription = s.SupDescription,
                            }).ToList();

            foreach (var service in services)
            {
                var Image = _context.Images.Where(i => i.ServiceId == service.ServiceId).FirstOrDefault();
                service.ImageUrl = Image.ImageUrl;
                var serviceOption = (from s in _context.Services 
                                     join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId 
                                     where s.ServiceId == service.ServiceId
                                     orderby so.Price ascending
                                     select so).FirstOrDefault();
                service.Price = serviceOption.Price;
            }

            return services;
        }
    }
}
