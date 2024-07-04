
using PetStoreProject.Areas.Admin.ViewModels;
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
                var id = 0;
                try
                {
                    id = _context.Discounts.Max(d => d.DiscountId);
                    id = id + 1;
                }
                catch (System.InvalidOperationException)
                {
                    id = 1;
                }
                var dis = new Models.Discount
                {
                    DiscountId = id,
                    Code = discount.Code,
                    StartDate = discount.StartDate,
                    EndDate = discount.EndDate,
                    DiscountTypeId = discount.DiscountTypeId,
                    Value = discount.Value,
                    CreatedAt = DateTime.Now.ToString(),
                    Description = discount.Description,
                    MaxValue = discount.MaxValue,
                    MinPurchase = discount.MinPurchase,
                    Quantity = discount.Quantity,
                    MaxUse = discount.MaxUse,
                    Used = 0,
                    Status = true
                };
                _context.Discounts.Add(dis);
                _context.SaveChanges();
                return _context.Discounts.Where(d => d.DiscountId == id)?.Select(d => d.Code).FirstOrDefault();
            }
        }

        public string Edit(Models.Discount discount)
        {
            var d = _context.Discounts.Find(discount.DiscountId);
            d.Status = false;
            _context.Discounts.Update(d);
            return Create(discount);
        }

        public DiscountViewModel GetDiscount(int id)
        {
            var discount = (from d in _context.Discounts
                            join dt in _context.DiscountTypes on d.DiscountTypeId equals dt.DiscountTypeId
                            where d.DiscountId == id
                            select new DiscountViewModel
                            {
                                Id = id,
                                Code = d.Code,
                                StartDate = d.StartDate,
                                EndDate = d.EndDate,
                                CreatedAt = d.CreatedAt,
                                DiscountType = new DiscountTypeViewModel
                                {
                                    Id = dt.DiscountTypeId,
                                    Name = dt.DiscountName
                                },
                                Value = d.Value,
                                MaxValue = d.MaxValue,
                                MinPurchase = d.MinPurchase,
                                Quantity = d.Quantity,
                                MaxUse = d.MaxUse,
                                Used = d.Used,
                                Description = d.Description
                            }).FirstOrDefault();
            return discount;
        }

        public List<DiscountViewModel> GetDiscounts()
        {
            var discounts = (from d in _context.Discounts
                             join dt in _context.DiscountTypes on d.DiscountTypeId equals dt.DiscountTypeId
                             select new DiscountViewModel
                             {
                                 Id = d.DiscountId,
                                 Code = d.Code,
                                 StartDate = d.StartDate,
                                 EndDate = d.EndDate,
                                 CreatedAt = d.CreatedAt,
                                 DiscountType = new DiscountTypeViewModel
                                 {
                                     Id = dt.DiscountTypeId,
                                     Name = dt.DiscountName
                                 },
                                 Value = d.Value,
                                 MaxValue = d.MaxValue,
                                 MinPurchase = d.MinPurchase,
                                 Quantity = d.Quantity,
                                 MaxUse = d.MaxUse,
                                 Used = d.Used,
                             }).ToList();
            var now = DateOnly.FromDateTime(DateTime.Now);
            foreach (var item in discounts)
            {
                if (item.DiscountType.Id == 3)
                {
                    item.Status = true;
                    item.StatusString = "Đang diễn ra";
                }
                else if (item.StartDate <= now && now <= item.EndDate && item.Status == true)
                {
                    if (item.Quantity < item.Used)
                    {
                        item.Status = false;
                        item.StatusString = "Hết lượt";
                    }
                    else
                    {
                        item.Status = true;
                        item.StatusString = "Đanh diễn ra";

                    }
                }
                else
                {
                    if (item.StartDate > now)
                    {
                        item.Status = true;
                        item.StatusString = "Chưa bắt đầu";
                    }
                    else
                    {
                        item.Status = false;
                        item.StatusString = "Đã kết thúc";
                    }
                }
            }
            return discounts;
        }
    }

}
