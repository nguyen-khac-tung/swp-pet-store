using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Shipper
{
    public interface IShipperRepository
    {
        public List<ShipperViewModel> GetTotalAccountShippers(ShipperFilterViewModel shipperFilerVM);

        public List<ShipperViewModel> GetAccountShippers(ShipperFilterViewModel shipperFilerVM, int pageIndex, int pageSize);

        public void DeleteShipperAccount(int id);
    }
}
