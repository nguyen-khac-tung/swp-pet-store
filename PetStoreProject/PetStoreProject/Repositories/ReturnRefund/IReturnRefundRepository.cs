namespace PetStoreProject.Repositories.ReturnRefund
{
    public interface IReturnRefundRepository
    {
        public void CreateReturnRefund(ViewModels.CreateReturnRefund returnRefund);

        public List<Models.ReturnRefund> GetReturnRefunds();
        public Task AddImageToReturnRefund(int returnId, ViewModels.CreateReturnRefund returnRefund);
    }
}
