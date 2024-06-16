using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;

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
    }
}
