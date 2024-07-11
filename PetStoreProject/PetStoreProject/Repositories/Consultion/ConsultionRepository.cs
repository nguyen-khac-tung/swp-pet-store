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
                Date = new DateOnly(),
                Status = false,
            };
            _dbContext.Consultations.Add(newConsultation);
            _dbContext.SaveChanges();
            return newConsultation.ConsultationId;
        }


        public List<ConsultionViewForAdmin> GetListConsultion()
        {
            var listConsultation = (from c in _dbContext.Consultations
                                    select new ConsultionViewForAdmin
                                    {
                                        Id = c.ConsultationId,
                                        Name = c.Name,  // Giả sử Name không bao giờ là NULL
                                        Email = c.Email ?? "No email provided",  // Nếu Email là NULL, sử dụng giá trị mặc định
                                        Phone = c.PhoneNumber ?? "No phone number",  // Tương tự như trên
                                        Title = c.Title ?? "No title",  // Tương tự như trên
                                        Date = c.Date,  // Giả sử Date không bao giờ là NULL
                                        Status = c.Status 
                                    }).ToList();

            return listConsultation;
        }
    }
}
