using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Consultion
{
    public class ConsultionRepository : IConsultionRepository
    {
        private readonly PetStoreDBContext _dbContext;

        public ConsultionRepository(PetStoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int CreateConsultion(CreateConsultion consultion)
        {
            var newConsultation = new Consultation
            {
                Name = consultion.CustomerName,
                Email = consultion.Email,
                PhoneNumber = consultion.Phone,
                Title = consultion.Title,
                Content = consultion.Content,
                Date = DateTime.Now,
                Status = false,
            };
            _dbContext.Consultations.Add(newConsultation);
            _dbContext.SaveChanges();
            return newConsultation.ConsultationId;
        }


        public List<ConsultionViewForAdmin> GetListConsultion()
        {
            var listConsultion = (from c in _dbContext.Consultations
                                  select new ConsultionViewForAdmin
                                  {
                                      Id = c.ConsultationId,
                                      Name = c.Name,
                                      Email = c.Email,
                                      Phone = c.PhoneNumber,
                                      Title = c.Title,
                                      Date = c.Date,
                                      Status = c.Status,
                                  }).ToList();
            return listConsultion;
        }
    }
}
