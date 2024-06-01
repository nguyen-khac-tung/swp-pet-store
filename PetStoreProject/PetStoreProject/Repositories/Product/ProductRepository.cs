using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;
using Attribute = PetStoreProject.Models.Attribute;

namespace PetStoreProject.Repositories.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly PetStoreDBContext _context;
        public ProductRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public ProductDetailViewModel GetDetail(int productId)
        {
            var product = (from p in _context.Products
                           join b in _context.Brands on p.BrandId equals b.BrandId
                           where p.ProductId == productId
                           select new ProductDetailViewModel
                           {
                               Name = p.Name,
                               Brand = b.Name,
                               Description = p.Description
                           }).FirstOrDefault();
            var images = (from po in _context.ProductOptions
                          join i in _context.Images on po.ImageId equals i.ImageId
                          where po.ProductId == productId
                          select new Image()
                          {
                              ImageId = i.ImageId,
                              ImageUrl = i.ImageUrl
                          }).Distinct().ToList();


            var productOptions = (from po in _context.ProductOptions
                                  join a in _context.Attributes on po.AttributeId equals a.AttributeId
                                  join p in _context.Products on po.ProductId equals p.ProductId
                                  join s in _context.Sizes on po.SizeId equals s.SizeId
                                  join i in _context.Images on po.ImageId equals i.ImageId
                                  where p.ProductId == productId
                                  select new ProductOptionViewModel
                                  {
                                      Id = po.ProductOptionId,
                                      size = new Size()
                                      {
                                          SizeId = s.SizeId,
                                          Name = s.Name
                                      },
                                      price = po.Price,
                                      img_url = i.ImageUrl,
                                      attribute = new Attribute()
                                      {
                                          AttributeId = a.AttributeId,
                                          Name = a.Name
                                      },
                                      IsSoldOut = po.IsSoldOut
                                  }).ToList();

            var attributes = productOptions.Select(a => a.attribute)
                               .GroupBy(a => a.AttributeId) // GroupBy theo ID hoặc thuộc tính duy nhất
                               .Select(g => g.First()) // Chọn phần tử đầu tiên từ mỗi nhóm
                               .ToList();

            var sizes = productOptions.Select(s => s.size)
                               .GroupBy(s => s.SizeId) // GroupBy theo ID hoặc thuộc tính duy nhất
                               .Select(g => g.First()) // Chọn phần tử đầu tiên từ mỗi nhóm
                               .ToList();
            bool isSoldOut = !(productOptions.Any(po => po.IsSoldOut == false));

            foreach (var image in images)
            {
                image.ImageUrl = formatUrl(image.ImageUrl);
            }
            product.ProductId = productId;
            product.attributes = attributes;
            product.sizes = sizes;
            product.images = images;
            product.productOption = productOptions;
            product.IsSoldOut = isSoldOut;

            return product;
        }

        public List<RelatedProductViewModel> getRelatedProduct(int productId)
        {
            var cateId = (from p in _context.Products where p.ProductId == productId select p.ProductCateId).FirstOrDefault();
            var products = (from po in _context.ProductOptions
                            join p in _context.Products on po.ProductId equals p.ProductId
                            join b in _context.Brands on p.BrandId equals b.BrandId
                            join i in _context.Images on po.ImageId equals i.ImageId
                            where p.ProductCateId == cateId && po.IsSoldOut == false
                            group new { po, p, b, i } by new { p.ProductId, p.Name, BrandName = b.Name } into g
                            select new RelatedProductViewModel
                            {
                                ProductId = g.Key.ProductId,
                                Name = g.Key.Name,
                                brand = g.Key.BrandName,
                                Price = g.Min(x => x.po.Price)
                            }).Take(12).ToList();
            foreach (var p in products)
            {
                var images = (from po in _context.ProductOptions
                              join i in _context.Images on po.ImageId equals i.ImageId
                              where po.ProductId == p.ProductId
                              select new Image()
                              {
                                  ImageId = i.ImageId,
                                  ImageUrl = i.ImageUrl
                              }).Distinct().ToList();
                foreach (var image in images)
                {
                    image.ImageUrl = formatUrl(image.ImageUrl);
                }

                p.images = images;
            }
            return products;
        }

        public string formatUrl(string url)
        {
            var img_id = url.Split('/')[url.Split('/').Length - 1];
            return "http://res.cloudinary.com/dvofidghe/image/upload/w_800,h_950/v1716019321/" + img_id;
        }

        public List<Models.Product> GetProductsByCategoriesAndProductCateId(List<int> categoriesIds, int productCateId)
        {

            List<Models.Product> products = (from p in _context.Products
                                             join pc in _context.ProductCategories on p.ProductCateId equals pc.ProductCateId
                                             where categoriesIds.Contains(pc.CategoryId)
                                             select p).ToList();
            if (productCateId != 0)
            {
                products = products.Where(p => p.ProductCateId == productCateId).ToList();
            }
            return products;
        }

        public List<ProductOptionViewModel> GetProductOptionsByProductId(int productId)
        {
            var productOptions = (from po in _context.ProductOptions
                                  join i in _context.Images on po.ImageId equals i.ImageId
                                  join a in _context.Attributes on po.AttributeId equals a.AttributeId
                                  join s in _context.Sizes on po.SizeId equals s.SizeId
                                  where po.ProductId == productId
                                  select new ProductOptionViewModel
                                  {
                                      Id = po.ProductOptionId,
                                      attribute = new Attribute
                                      {
                                          AttributeId = a.AttributeId,
                                          Name = a.Name,
                                          Type = a.Type,
                                      },
                                      size = new Size
                                      {
                                          SizeId = s.SizeId,
                                          Name = s.Name
                                      },
                                      price = po.Price,
                                      img_url = i.ImageUrl,

                                      IsSoldOut = po.IsSoldOut
                                  }).ToList();
            return productOptions;
        }

        public List<Image> GetImagesByProductId(int productId)
        {
            var images = (from i in _context.Images
                          join po in _context.ProductOptions on i.ImageId equals po.ImageId
                          where po.ProductId == productId
                          select i).ToList();
            return images;
        }

        public List<Attribute> GetAttributesByProductId(int productId)
        {
            var attributes = (from a in _context.Attributes
                              join po in _context.ProductOptions on a.AttributeId equals po.AttributeId
                              where po.ProductId == productId
                              select a).ToList();
            return attributes;
        }

        public List<Size> GetSizesByProductId(int productId)
        {
            var sizes = (from s in _context.Sizes
                         join po in _context.ProductOptions on s.SizeId equals po.SizeId
                         where po.ProductId == productId
                         select s).ToList();
            return sizes;
        }

        public Brand GetBrandByProductId(int productId)
        {
            var brand = (from b in _context.Brands
                         join p in _context.Products on b.BrandId equals p.BrandId
                         where p.ProductId == productId
                         select b).FirstOrDefault();
            return brand;
        }

        public List<Brand> GetBrandsByCategoryIdsAndProductCateId(List<int> categoryIds, int productCateId)
        {

            var brands = (from b in _context.Brands
                          join p in _context.Products on b.BrandId equals p.BrandId
                          join pc in _context.ProductCategories on p.ProductCateId equals pc.ProductCateId
                          where categoryIds.Contains(pc.CategoryId)
                          select b).Distinct().ToList();
            if (productCateId != 0)
            {
                brands = (from b in brands
                          join p in _context.Products on b.BrandId equals p.BrandId
                          where p.ProductCateId == productCateId
                          select b).ToList();
            }
            brands = brands.Distinct().ToList();
            return brands;
        }

        public List<Size> GetSizesByCategoryIdsAndProductCateId(List<int> categoryIds, int productCateId)
        {

            var sizes = (from s in _context.Sizes
                         join po in _context.ProductOptions on s.SizeId equals po.SizeId
                         join p in _context.Products on po.ProductId equals p.ProductId
                         join pc in _context.ProductCategories on p.ProductCateId equals pc.ProductCateId
                         where categoryIds.Contains(pc.CategoryId)
                         select s).Distinct().ToList();
            if (productCateId != 0)
            {
                sizes = (from s in sizes
                         join po in _context.ProductOptions on s.SizeId equals po.SizeId
                         join p in _context.Products on po.ProductId equals p.ProductId
                         where p.ProductCateId == productCateId
                         select s).Distinct().ToList();
            }
            sizes = sizes.Distinct().ToList();
            return sizes;
        }




        public List<ProductDetailViewModel> GetProductDetailResponse(List<Models.Product> products)
        {

            var productDetails = products.Select(p => new ProductDetailViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Brand = GetBrandByProductId(p.ProductId).Name,
                Description = p.Description,
                productOption = GetProductOptionsByProductId(p.ProductId),
                //images = GetImagesByProductId(p.ProductId),
                //attributes = GetAttributesByProductId(p.ProductId),
                //sizes = GetSizesByProductId(p.ProductId)

            }).ToList();
            return productDetails;
        }


        public List<ProductDetailViewModel> GetProductDetailRequest(List<Models.Product> products)
        {
            var productDetails = products.Select(p => new ProductDetailViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                productOption = GetProductOptionsByProductId(p.ProductId),

            }).ToList();
            return productDetails;
        }

        public List<ProductDetailViewModel> GetProductDetailAccessoriesRequest(List<int> cateId, int productCateId)
        {

            var products = GetProductsByCategoriesAndProductCateId(cateId, productCateId);
            return GetProductDetailRequest(products);
        }
        public List<ProductDetailViewModel> GetProductDetailAccessoriesResponse(List<int> cateId, int productCateId)

        {

            var products = GetProductsByCategoriesAndProductCateId(cateId, productCateId);
            return GetProductDetailResponse(products);
        }

        public List<ProductDetailViewModel> GetProductDetailFoodsRequest(List<int> cateId, int productCateId)
        {
            var products = GetProductsByCategoriesAndProductCateId(cateId, productCateId);
            return GetProductDetailRequest(products);
        }

        public List<ProductDetailViewModel> GetProductDetailFoodsResponse(List<int> cateId, int productCateId)
        {
            var products = GetProductsByCategoriesAndProductCateId(cateId, productCateId);
            return GetProductDetailResponse(products);
        }

        public List<int> GetProductIDInWishList(int customerID)
        {
            var list = (from c in _context.Customers
                        where c.CustomerId == customerID
                        from p in c.Products
                        select p.ProductId
                    ).ToList();
            return list;
        }

        public void AddToFavorites(int customerID, int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerID);
            customer.Products.Add(product);
            _context.SaveChanges();
        }

        public void RemoveFromFavorites(int customerID, int productId)
        {
            var customer = _context.Customers.Include(c => c.Products)
                                             .FirstOrDefault(c => c.CustomerId == customerID);
            var product = customer.Products.FirstOrDefault(p => p.ProductId == productId);
            customer.Products.Remove(product);
            _context.SaveChanges();
        }

        public List<SearchViewModel> GetListProductsByKeyWords(string key)
        {
            //Search with key accented
            var listSearch = (from p in _context.Products
                              join po in _context.ProductOptions on p.ProductId equals po.ProductId into productOptions
                              from po in productOptions.OrderBy(po => po.ProductOptionId).Take(1)
                              join i in _context.Images on po.ImageId equals i.ImageId
                              where p.Name.Contains(key)
                              orderby p.Name.IndexOf(key)
                              select new SearchViewModel
                              {
                                  ProductId = p.ProductId,
                                  ProductName = p.Name,
                                  price = po.Price,
                                  img_url = i.ImageUrl
                              }).ToList();


            if (listSearch.IsNullOrEmpty())
            {
                string removeAccentKey = RemoveVietnameseAccents(key);

                //Search with key witfout accented 
                List<SearchViewModel> listP = (from p in _context.Products
                                               join po in _context.ProductOptions on p.ProductId equals po.ProductId into productOptions
                                               from po in productOptions.OrderBy(po => po.ProductOptionId).Take(1)
                                               join i in _context.Images on po.ImageId equals i.ImageId
                                               select new SearchViewModel
                                               {
                                                   ProductId = p.ProductId,
                                                   ProductName = RemoveVietnameseAccents(p.Name),
                                                   price = po.Price,
                                                   img_url = i.ImageUrl
                                               }).ToList();

                listSearch = listP.Where(p => p.ProductName.Contains(removeAccentKey, StringComparison.OrdinalIgnoreCase))
                                  .OrderBy(p => p.ProductName.IndexOf(removeAccentKey, StringComparison.OrdinalIgnoreCase))
                                  .ToList();
                foreach (var item in listSearch)
                {
                    item.ProductName = _context.Products
                                                .Where(p => p.ProductId == item.ProductId)
                                                .Select(p => p.Name)
                                                .SingleOrDefault() ?? "Default Product Name";
                }
            }
            return listSearch;
        }
        private static string RemoveVietnameseAccents(string input)
        {
            string[] vietnameseSigns = new string[]
            {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
            };

            for (int i = 1; i < vietnameseSigns.Length; i++)
            {
                for (int j = 0; j < vietnameseSigns[i].Length; j++)
                    input = input.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
            }
            return input;
        }
    }
        internal record NewRecord(int ProductId, string Name, string Item);
}
