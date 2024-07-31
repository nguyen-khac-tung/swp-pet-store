using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Shipper
{
    public class ShipperRepository : IShipperRepository
    {
        public readonly PetStoreDBContext _context;

        public ShipperRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public List<ShipperViewModel> GetTotalAccountShippers(ShipperFilterViewModel shipperFilerVM)
        {
            string? shipperName = string.IsNullOrEmpty(shipperFilerVM.Name) ? null : shipperFilerVM.Name;
            int? districtID = string.IsNullOrEmpty(shipperFilerVM.DistrictId) ? null : Int32.Parse(shipperFilerVM.DistrictId);
            bool? status = string.IsNullOrEmpty(shipperFilerVM.Status) ? null : bool.Parse(shipperFilerVM.Status);

            var shippers = (from s in _context.Shippers
                            join a in _context.Accounts on s.AccountId equals a.AccountId
                            join d in _context.Districts on s.ShipperId equals d.ShipperId
                            where (shipperName == null || s.FullName.Contains(shipperName))
                            && (districtID == null || d.DistrictId == districtID)
                            && (status == null || s.IsDelete == status)
                            select new ShipperViewModel
                            {
                                AccountId = s.AccountId,
                                ShipperId = s.ShipperId,
                                FullName = s.FullName,
                                Gender = s.Gender,
                                Phone = s.Phone,
                                DoB = s.DoB,
                                Address = s.Address,
                                Email = s.Email,
                                Role = _context.Roles.FirstOrDefault(r => r.RoleId == a.RoleId),
                                IsDelete = s.IsDelete,
                            }).Distinct().ToList();

            return shippers;
        }

        public List<ShipperViewModel> GetAccountShippers(ShipperFilterViewModel shipperFilerVM, int pageIndex, int pageSize)
        {
            List<ShipperViewModel> shippers = GetTotalAccountShippers(shipperFilerVM);

            if (shippers != null)
            {
                foreach (var shipper in shippers)
                {
                    shipper.Districts = _context.Districts.Where(d => d.ShipperId == shipper.ShipperId).ToList();
                    shipper.TotalDeliveredQuantity = _context.Orders.Where(o => o.ShipperId == shipper.ShipperId).Count();
                }

                if (shipperFilerVM.SortName != null)
                {
                    if (shipperFilerVM.SortName == "Ascending")
                        shippers = shippers.OrderBy(s => s.FullName).ToList();
                    else
                        shippers = shippers.OrderByDescending(s => s.FullName).ToList();
                }

                if (shipperFilerVM.SortDeliveryQuantity != null)
                {
                    if (shipperFilerVM.SortDeliveryQuantity == "Ascending")
                        shippers = shippers.OrderBy(s => s.TotalDeliveredQuantity).ToList();
                    else
                        shippers = shippers.OrderByDescending(s => s.TotalDeliveredQuantity).ToList();
                }
            }

            var listDisplay = shippers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return listDisplay;
        }

        public void DeleteShipperAccount(int id)
        {
            var shipper = _context.Shippers.FirstOrDefault(s => s.ShipperId == id);
            shipper.IsDelete = true;

            var accountShipper = _context.Accounts.FirstOrDefault(s => s.AccountId == shipper.AccountId);
            accountShipper.IsDelete = true;

            _context.SaveChanges();
        }
    }
}
