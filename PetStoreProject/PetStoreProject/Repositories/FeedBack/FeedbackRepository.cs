using Microsoft.EntityFrameworkCore;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.FeedBack
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly PetStoreDBContext _dbContext;

        public FeedbackRepository(PetStoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<FeedBackViewModels> GetListFeedBack()
        {
            var list = (from fb in _dbContext.Feedbacks
                        join p in _dbContext.Products on fb.ProductId equals p.ProductId
                        join resp in _dbContext.ResponseFeedbacks on fb.FeedbackId equals resp.FeedbackId into gj
                        from subResp in gj.DefaultIfEmpty()
                        select new FeedBackViewModels
                        {
                            FeedBackId = fb.FeedbackId,
                            ProductName = p.Name,
                            CustomerName = fb.Name,
                            Rating = fb.Rating,
                            Content = fb.Content,
                            CreatedDate = fb.DateCreated,
                            Status = fb.Status,
                            ContentResponse = subResp != null ? subResp.Content : null
                        }).ToList();
            return list;
        }

        public List<FeedbackViewModels> GetListFeedBackForService(int serviceId)
        {
            var listFeedback = (from fb in _dbContext.Feedbacks
                                where fb.ServiceId == serviceId
                                join respfb in _dbContext.ResponseFeedbacks on fb.FeedbackId equals respfb.FeedbackId into feedbackResponses
                                from resp in feedbackResponses.DefaultIfEmpty()
                                join e in _dbContext.Employees on resp.EmployeeId equals e.EmployeeId into employeeResponses
                                from emp in employeeResponses.DefaultIfEmpty()
                                select new FeedbackViewModels
                                {
                                    CustomerName = fb.Name,
                                    Rating = fb.Rating,
                                    Content = fb.Content,
                                    EmployeeName = emp != null ? emp.FullName : null,
                                    ContentResponse = resp != null ? resp.Content : null,
                                    DateCreated = fb.DateCreated,
                                    DateResp = (DateTime)(resp != null ? resp.DateCreated : (DateTime?)null)
                                }).ToList();


            return listFeedback;
        }

    }
}
