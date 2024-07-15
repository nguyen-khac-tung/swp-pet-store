
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using X.PagedList;

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
                return "Fail";
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
                return "Success";
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

        public IPagedList<DiscountViewModel> GetDiscounts(int page, int pageSize)
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
                                 Status = d.Status
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
                        item.StatusString = "Đang diễn ra";

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
            return discounts.ToPagedList(page, pageSize);
        }

        public List<DiscountViewModel> GetDiscounts(double total_amount, string email)
        {
            var now = DateOnly.FromDateTime(DateTime.Now);
            var discounts = new List<DiscountViewModel>();
            bool isFirstOrder = true;

            if (email == null)
            {
                discounts = (from d in _context.Discounts
                             join dt in _context.DiscountTypes on d.DiscountTypeId equals dt.DiscountTypeId
                             where d.EndDate >= now && d.Status == true && d.StartDate <= now && dt.DiscountTypeId != 3
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
                                 Status = d.Status
                             }).ToList();
                isFirstOrder = false;
            }
            else
            {
                discounts = (from d in _context.Discounts
                             join dt in _context.DiscountTypes on d.DiscountTypeId equals dt.DiscountTypeId
                             where d.EndDate >= now && d.Status == true && d.StartDate <= now
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
                                 Status = d.Status
                             }).ToList();
                var customerId = _context.Customers.Where(c => c.Email == email).FirstOrDefault().CustomerId;
                var order = _context.Orders.Where(o => o.CustomerId == customerId).FirstOrDefault();
                if (order != null)
                {
                    isFirstOrder = false;
                }
            }

            foreach (var item in discounts)
            {
                if (item.DiscountType.Id == 3 && !isFirstOrder)
                {
                    discounts.Remove(item);
                }
                else if (item.Used >= item.MaxUse)
                {
                    item.Status = false;
                    item.StatusString = "Đã hết lượt sử dụng";
                }
                else if ((double)item.MinPurchase > total_amount)
                {
                    item.Status = false;
                    item.StatusString = "Mua thêm " + ((double)item.MinPurchase - total_amount).ToString("#,###.###") + " VND sản phẩm để sử dụng";
                }
                else
                {
                    item.Status = true;
                    item.Reduce = item.Value / 100 * (decimal)total_amount > item.MaxValue ? item.MaxValue : item.Value / 100 * (decimal)total_amount;
                    item.Title = "-" + ((decimal)item.Reduce).ToString("#,###") + " VND";
                }
            }
            return discounts;
        }

        public float GetDiscountPrice(double total_amount, int discountId)
        {
            var item = _context.Discounts.Find(discountId);
            var reduce = item.Value / 100 * (decimal)total_amount > item.MaxValue ? item.MaxValue : item.Value / 100 * (decimal)total_amount;
            return (float)reduce;
        }

        public void DeleteDiscount(int id)
        {
            var discount = _context.Discounts.Find(id);
            discount.Status = false;
            _context.SaveChanges();
        }
    }

}
