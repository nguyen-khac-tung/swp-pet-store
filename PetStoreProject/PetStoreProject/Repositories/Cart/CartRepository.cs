using PetStoreProject.Models;
using PetStoreProject.ViewModels;
using Attribute = PetStoreProject.Models.Attribute;

namespace PetStoreProject.Repositories.Cart
{
    public class CartRepository : ICartRepository
    {
        private readonly PetStoreDBContext _context;

        public CartRepository(PetStoreDBContext context)
        {
            _context = context;
        }


        public CartItemViewModel GetCartItemVM(int productOptionId)
        {
            var productOption = (from po in _context.ProductOptions
                                 join p in _context.Products on po.ProductId equals p.ProductId
                                 join a in _context.Attributes on po.AttributeId equals a.AttributeId
                                 join s in _context.Sizes on po.SizeId equals s.SizeId
                                 join i in _context.Images on po.ImageId equals i.ImageId
                                 where po.ProductOptionId == productOptionId
                                 select new CartItemViewModel()
                                 {
                                     ProductOptionId = productOptionId,
                                     Name = p.Name,
                                     Attribute = new Attribute()
                                     {
                                         Name = a.Name,
                                         AttributeId = a.AttributeId
                                     },
                                     Size = new Size()
                                     {
                                         SizeId = s.SizeId,
                                         Name = s.Name
                                     },
                                     Price = po.Price,
                                     ImgUrl = i.ImageUrl,
                                     Quantity = 1,
                                     ProductId = p.ProductId
                                 }).FirstOrDefault();
            return productOption;
        }
    }
}
