
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Discount
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly PetStoreDBContext _context;

        public DiscountRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public string Create(Models.Discount discount)
        {
            var duplicate = _context.Discounts.Any(d => d.Code == discount.Code && !((d.StartDate > discount.EndDate) || (d.EndDate < discount.StartDate)));
            if (duplicate)
            {
                return "Không thể có 2 mã giảm giá trùng nhau tại một thời điểm";
            }
            else
            {
                var id = _context.Discounts.Max(d => d.DiscountId);
                if (id == null)
                {
                    id = 1;
                }
                else
                {
                    id = id + 1;
                }
                var dis = new Models.Discount
                {
                    DiscountId = id,
                    Code = discount.Code,
                    StartDate = discount.StartDate,
                    EndDate = discount.EndDate,
                    DiscountTypeId = discount.DiscountTypeId,
                    Value = discount.Value,
                    CreatedAt = BitConverter.GetBytes(DateTime.Now.Ticks),
                    Description = discount.Description,
                    MaxValue = discount.MaxValue,
                    MinPurchase = discount.MinPurchase,
                    Quantity = discount.Quantity,
                    MaxUse = discount.MaxUse,
                    Used = 0
                };
                _context.Discounts.Add(dis);
                _context.SaveChanges();
                return _context.Discounts.Where(d => d.DiscountId == 1).Select(d => d.CreatedAt).FirstOrDefault().ToString();
            }
        }
    }
}
