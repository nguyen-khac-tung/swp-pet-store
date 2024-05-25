

using Microsoft.EntityFrameworkCore;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.WishList
{
    public class WishListRepository : IWishListRepository
    {
        private readonly PetStoreDBContext _dbContext;

        public WishListRepository(PetStoreDBContext dBContext) => _dbContext = dBContext;

        public Image GetUrlImgByProductID(int productId)
        {
            var image = (from po in _dbContext.ProductOptions
                         where po.ProductId == productId
                         join img in _dbContext.Images on po.ImageId equals img.ImageId
                         select img
                       ).FirstOrDefault();
            return image;
        }

        public Brand GetBrandByProductId(int productId)
        {
            var brand = (from p in _dbContext.Products
                         where p.ProductId == productId
                         join b in _dbContext.Brands on p.BrandId equals b.BrandId
                         select b).FirstOrDefault();
            return brand;
        }

        public float GetPriceByByDefault(int productID)
        {
            return (from p in _dbContext.ProductOptions
                    where p.ProductId == productID
                    select p.Price).FirstOrDefault();
        }
        public string GetProductNameByID(int productID)
        {
            var productName = (from p in _dbContext.Products
                               where p.ProductId == productID
                               select p.Name).FirstOrDefault();
            return productName;
        }


        public List<WishListVM> wishListVMs(int customerId)
        {
            List<WishListVM> listVMs = new List<WishListVM>();
            var wishtList = _dbContext.FavouriteLists.Where(x => x.CustomerId == customerId).ToList();
            foreach (var w in wishtList)
            {
                WishListVM wl = new WishListVM();
                wl.produtcID = w.ProductId;
                wl.productName = GetProductNameByID (w.ProductId);
                wl.price = GetPriceByByDefault(w.ProductId);
                wl.brandName = GetBrandByProductId (w.ProductId).Name;
                wl.img_url = GetUrlImgByProductID(w.ProductId).ImageUrl;
                listVMs.Add(wl);
            }
            return listVMs;
        }

		public void DeleteFromWishList(int customerId, int productId)
		{
			var favoriteItem = _dbContext.FavouriteLists
										  .FirstOrDefault(f => f.CustomerId == customerId && f.ProductId == productId);

			if (favoriteItem != null)
			{
				_dbContext.FavouriteLists.Remove(favoriteItem);
				_dbContext.SaveChanges(); 
			}
		}

	}
}
