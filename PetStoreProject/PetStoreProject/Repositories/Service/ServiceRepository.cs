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

        public ServiceDetailViewModel GetServiceDetail(int serviceId)
        {
            var service = (from s in _context.Services
                           where s.ServiceId == serviceId
                           select new ServiceDetailViewModel
                           {
                               ServiceId = s.ServiceId,
                               Name = s.Name,
                               Type = s.Type,
                               Description = s.Description,
                               IsDelete = s.IsDelete,
                           }).FirstOrDefault();

            var images = (from s in _context.Services
                          join i in _context.Images on s.ServiceId equals i.ServiceId
                          where s.ServiceId == serviceId
                          select i).ToList();

            var petTypes = (from s in _context.Services
                            join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                            where s.ServiceId == serviceId
                            select so.PetType).Distinct().ToList();

            service.Images = images;
            service.PetTypes = petTypes;
            return service;
        }

        public ServiceOptionViewModel GetFistServiceOption(int serviceId)
        {
            var petType = (from s in _context.Services
                           join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                           where s.ServiceId == serviceId
                           select so.PetType).FirstOrDefault();

            return GetFirstServiceAndListWeightOfPetType(serviceId, petType);
        }

        public ServiceOptionViewModel GetFirstServiceAndListWeightOfPetType(int serviceId, string petType)
        {
            var firstServiceOption = (from so in _context.ServiceOptions
                                      where so.ServiceId == serviceId && so.PetType == petType
                                      select so).FirstOrDefault();
            var listWeight = (from so in _context.ServiceOptions
                              where so.ServiceId == serviceId && so.PetType == petType
                              orderby so.ServiceOptionId ascending
                              select so.Weight).ToList();
            return new ServiceOptionViewModel
            {
                ServiceId = firstServiceOption.ServiceId,
                ServiceOptionId = firstServiceOption.ServiceOptionId,
                PetType = firstServiceOption.PetType,
                Weight = firstServiceOption.Weight,
                Price = firstServiceOption.Price,
                Weights = listWeight
            };
        }

        public ServiceOptionViewModel GetNewServiceOptionBySelectWeight(int serviceId, string petType, string weight)
        {
            var serviceOption = (from so in _context.ServiceOptions
                                 where so.ServiceId == serviceId && so.PetType == petType && so.Weight == weight
                                 select new ServiceOptionViewModel
                                 {
                                     ServiceOptionId = so.ServiceOptionId,
                                     Price = so.Price,
                                 }).FirstOrDefault();

            return serviceOption;
        }

        public List<ServiceViewModel> GetOtherServices(int serviceId)
        {
            var services = (from s in _context.Services
                            where s.IsDelete == false && s.ServiceId != serviceId
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
