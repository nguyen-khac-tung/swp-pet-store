using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Consultion
{
    public class ConsultionRepository : IConsultionRepository
    {
        private readonly PetStoreDBContext _dbContext;
        private readonly EmailService _emailService;

        public ConsultionRepository(PetStoreDBContext dbContext, EmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public int CreateConsultion(ConsultionCreateRequestViewModel consultion)
        {
            var newConsultation = new Consultation
            {
                Name = consultion.CustomerName,
                Email = consultion.Email,
                PhoneNumber = consultion.Phone,
                Title = consultion.Title,
                Content = consultion.Content,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Status = false,
            };
            _dbContext.Consultations.Add(newConsultation);
            _dbContext.SaveChanges();
            return newConsultation.ConsultationId;
        }

        public ConsultionViewForAdmin GetDetail(int id)
        {
            var consultion = _dbContext.Consultations.Find(id);
            if (consultion == null)
            {
                return null;
            }
            return new ConsultionViewForAdmin
            {
                Id = consultion.ConsultationId,
                Name = consultion.Name,
                Email = consultion.Email,
                Phone = consultion.PhoneNumber,
                Title = consultion.Title,
                Date = consultion.Date,
                Status = consultion.Status,
                Content = consultion.Content
            };
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

        public void Reply(int id, string message)
        {
            var consultation = _dbContext.Consultations.Find(id);
            var email = consultation.Email;
            var subject = "Trả lời yêu cầu";
            var body = consultation.Content + ": " + message;
            consultation.Status = true;
            _dbContext.SaveChanges();
            _emailService.SendEmail(email, subject, body);
        }
    }
}
