using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Attribute;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.Image;
using PetStoreProject.Repositories.Size;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly PetStoreDBContext _context;
        private readonly IImageRepository _image;
        private readonly IBrandRepository _brand;
        private readonly ISizeRepository _size;
        private readonly IAttributeRepository _attribute;

        public ProductRepository(PetStoreDBContext context, IImageRepository image,
            IBrandRepository brand, ISizeRepository size, IAttributeRepository attribute)
        {
            _context = context;
            _image = image;
            _brand = brand;
            _size = size;
            _attribute = attribute;
        }
        public List<FeedbackViewModels> GetListFeedBack(int productId)
        {
            var listFeedback = (from fb in _context.Feedbacks
                                where fb.ProductId == productId
                                join respfb in _context.ResponseFeedbacks on fb.FeedbackId equals respfb.FeedbackId into feedbackResponses
                                from resp in feedbackResponses.DefaultIfEmpty()
                                join e in _context.Employees on resp.EmployeeId equals e.EmployeeId into employeeResponses
                                from emp in employeeResponses.DefaultIfEmpty()
                                select new FeedbackViewModels
                                {
                                    CustomerName = fb.Name,
                                    Rating = fb.Rating,
                                    Content = fb.Content,
                                    EmployeeName = emp != null ? emp.FullName : null,
                                    ContentResponse = resp != null ? resp.Content : null,
                                    DateCreated = fb.DateCreated,
                                    DateResp = (DateTime)(resp != null ? resp.DateCreated : (DateTime?)null)
                                }).ToList();


            return listFeedback;
        }

        public ProductDetailViewModel GetDetail(int productId)
        {
            var product = (from p in _context.Products
                           join b in _context.Brands on p.BrandId equals b.BrandId
                           join pc in _context.ProductCategories on p.ProductCateId equals pc.ProductCateId
                           where p.ProductId == productId
                           select new ProductDetailViewModel
                           {
                               Name = p.Name,
                               Brand = b.Name,
                               BrandId = b.BrandId,
                               Description = p.Description,
                               ProductCateId = pc.ProductCateId
                           }).FirstOrDefault();
            var images = (from po in _context.ProductOptions
                          join i in _context.Images on po.ImageId equals i.ImageId
                          where po.ProductId == productId
                          select new PetStoreProject.Models.Image()
                          {
                              ImageId = i.ImageId,
                              ImageUrl = i.ImageUrl
                          }).Distinct().ToList();


            var productOptions = (from po in _context.ProductOptions
                                  join a in _context.Attributes on po.AttributeId equals a.AttributeId
                                  join p in _context.Products on po.ProductId equals p.ProductId
                                  join s in _context.Sizes on po.SizeId equals s.SizeId
                                  join i in _context.Images on po.ImageId equals i.ImageId
                                  where p.ProductId == productId && po.IsDelete == false
                                  select new ProductOptionViewModel
                                  {
                                      Id = po.ProductOptionId,
                                      size = new PetStoreProject.Models.Size()
                                      {
                                          SizeId = s.SizeId,
                                          Name = s.Name
                                      },
                                      price = po.Price,
                                      img_url = i.ImageUrl,
                                      attribute = new PetStoreProject.Models.Attribute()
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
            var max_price = 0.0;
            foreach (var po in productOptions)
            {
                po.img_url = formatUrl(po.img_url);
                if (po.price > max_price)
                {
                    max_price = po.price;
                }
            }

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

            product.Promotion = GetPromotionForProduct(product.BrandId, product.ProductCateId);

            return product;
        }

        private Models.Promotion GetPromotionForProduct(int brandId, int productCateId)
        {
            var now = DateTime.Now;
            var promotions = (from p in _context.Promotions
                              where (p.BrandId == 0 || p.BrandId == brandId)
                                 && (p.ProductCateId == 0 || p.ProductCateId == productCateId)
                                 && p.Status == true
                              select p).ToList();

            if (promotions.Count != 0)
            {
                int max = 0;
                Models.Promotion p = new Models.Promotion();
                foreach (var item in promotions)
                {
                    if (item.Value > max && (DateTime.Parse(item.StartDate) <= now && DateTime.Parse(item.EndDate) >= now))
                    {
                        max = (int)item.Value;
                        p = item;
                    }
                }
                return p;
            }
            return null;
        }

        public List<RelatedProductViewModel> getRelatedProduct(int productId)
        {
            var cateId = (from p in _context.Products where p.ProductId == productId select p.ProductCateId).FirstOrDefault();
            var products = (from po in _context.ProductOptions
                            join p in _context.Products on po.ProductId equals p.ProductId
                            join b in _context.Brands on p.BrandId equals b.BrandId
                            join i in _context.Images on po.ImageId equals i.ImageId
                            where p.ProductCateId == cateId && po.IsSoldOut == false && p.IsDelete == false
                            group new { po, p, b, i } by new { p.ProductId, p.Name, BrandName = b.Name, b.BrandId, p.ProductCateId } into g
                            select new RelatedProductViewModel
                            {
                                ProductId = g.Key.ProductId,
                                Name = g.Key.Name,
                                brand = g.Key.BrandName,
                                Price = g.Min(x => x.po.Price),
                                BrandId = g.Key.BrandId,
                                ProductCateId = g.Key.ProductCateId

                            }).Take(12).ToList();
            foreach (var p in products)
            {
                var images = (from po in _context.ProductOptions
                              join i in _context.Images on po.ImageId equals i.ImageId
                              where po.ProductId == p.ProductId
                              select new PetStoreProject.Models.Image()
                              {
                                  ImageId = i.ImageId,
                                  ImageUrl = i.ImageUrl
                              }).Distinct().ToList();
                foreach (var image in images)
                {
                    image.ImageUrl = formatUrl(image.ImageUrl);
                }

                p.Promotion = GetPromotionForProduct(p.BrandId, p.ProductCateId);

                p.images = images;
            }
            return products;
        }

        public string formatUrl(string url)
        {
            var img_id = url.Split('/')[url.Split('/').Length - 1];
            return "http://res.cloudinary.com/dvofidghe/image/upload/w_800,h_950/" + img_id;
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
                                      attribute = new PetStoreProject.Models.Attribute
                                      {
                                          AttributeId = a.AttributeId,
                                          Name = a.Name,
                                          Type = a.Type,
                                      },
                                      size = new PetStoreProject.Models.Size
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

        public PetStoreProject.Models.Brand GetBrandByProductId(int productId)
        {
            var brand = (from b in _context.Brands
                         join p in _context.Products on b.BrandId equals p.BrandId
                         where p.ProductId == productId
                         select b).FirstOrDefault();
            return brand;
        }

        public List<PetStoreProject.Models.Brand> GetBrandsByCategoryIdsAndProductCateId(List<int> categoryIds, int productCateId)
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

        public List<PetStoreProject.Models.Size> GetSizesByCategoryIdsAndProductCateId(List<int> categoryIds, int productCateId)
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

        public List<ProductDetailViewModel> GetProductDetail(List<int> cateId, int productCateId)
        {
            var products = GetProductsByCategoriesAndProductCateId(cateId, productCateId);

            var promotionDefault = _context.Promotions
                .Where(pro => pro.BrandId == 0 && pro.ProductCateId == 0 && pro.Status == true)
                .OrderByDescending(pro => pro.Value)
                .FirstOrDefault();

            var productDetails = products.Select(p =>
            {
                var brand = GetBrandByProductId(p.ProductId);
                var promotion = GetPromotionForProduct(brand.BrandId, p.ProductCateId);

                return new ProductDetailViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Brand = brand.Name,
                    Description = p.Description,
                    productOption = GetProductOptionsByProductId(p.ProductId),
                    Promotion = promotion,
                };
            }).ToList();

            return productDetails;
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

        public SearchResultViewModel GetListProductsByKeyWords(string key, int page)
        {
            SearchResultViewModel searchResultViewModel = new SearchResultViewModel();

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


            var pagedResult = listSearch.Skip((page - 1) * 20)
                           .Take(20)
                           .ToList();
            searchResultViewModel.TotalResults = listSearch.Count;
            searchResultViewModel.Results = pagedResult;

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
                pagedResult = listSearch.Skip((page - 1) * 20)
                           .Take(20)
                           .ToList();

                searchResultViewModel.TotalResults = listSearch.Count;
                searchResultViewModel.Results = pagedResult;

                foreach (var item in pagedResult)
                {
                    item.ProductName = _context.Products
                                                .Where(p => p.ProductId == item.ProductId)
                                                .Select(p => p.Name)
                                                .SingleOrDefault() ?? "Default Product Name";
                }
            }
            return searchResultViewModel;
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

        public int GetNumberOfDogFoods()
        {
            var count = (from c in _context.Categories
                         join pc in _context.ProductCategories on c.CategoryId equals pc.CategoryId
                         join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                         where c.CategoryId == 1 || c.CategoryId == 3
                         select p.ProductId).Count();
            return count;
        }

        public int GetNumberOfDogAccessories()
        {
            var count = (from c in _context.Categories
                         join pc in _context.ProductCategories on c.CategoryId equals pc.CategoryId
                         join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                         where c.CategoryId == 2 || c.CategoryId == 5
                         select p.ProductId).Count();
            return count;
        }

        public int GetNumberOfCatFoods()
        {
            var count = (from c in _context.Categories
                         join pc in _context.ProductCategories on c.CategoryId equals pc.CategoryId
                         join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                         where c.CategoryId == 1 || c.CategoryId == 4
                         select p.ProductId).Count();
            return count;
        }

        public int GetNumberOfCatAccessories()
        {
            var count = (from c in _context.Categories
                         join pc in _context.ProductCategories on c.CategoryId equals pc.CategoryId
                         join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                         where c.CategoryId == 2 || c.CategoryId == 6
                         select p.ProductId).Count();
            return count;
        }

        public List<HomeProductViewModel> GetProductsOfHome(int cateId, int? productCateId)
        {
            List<HomeProductViewModel> products = new List<HomeProductViewModel>();
            if (productCateId == null)
            {
                products = (from c in _context.Categories
                            join pc in _context.ProductCategories on c.CategoryId equals pc.CategoryId
                            join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                            where c.CategoryId == cateId
                            select new HomeProductViewModel
                            {
                                ProductId = p.ProductId,
                                ProductName = p.Name,
                                BrandId = p.BrandId
                            }).ToList();
            }
            else
            {
                products = (from c in _context.Categories
                            join pc in _context.ProductCategories on c.CategoryId equals pc.CategoryId
                            join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                            where c.CategoryId == cateId && pc.ProductCateId == productCateId
                            select new HomeProductViewModel
                            {
                                ProductId = p.ProductId,
                                ProductName = p.Name,
                                BrandId = p.BrandId
                            }).ToList();

            }
            foreach (var product in products)
            {
                product.Promotion = GetPromotionForProduct(product.BrandId, productCateId ?? 0);
            }
            return products;
        }

        public HomeProductViewModel GetImageAndPriceOfHomeProduct(HomeProductViewModel product)
        {
            var productOptions = (from p in _context.Products
                                  join po in _context.ProductOptions on p.ProductId equals po.ProductId
                                  join i in _context.Images on po.ImageId equals i.ImageId
                                  where p.ProductId == product.ProductId
                                  select new
                                  {
                                      ImageUrl = i.ImageUrl,
                                      Price = po.Price,
                                      IsSoldOut = po.IsSoldOut
                                  }).ToList();
            foreach (var productOption in productOptions)
            {
                if (productOption.IsSoldOut == true)
                {
                    return null;
                }
            }

            product.Price = productOptions[0].Price;
            product.ImageUrl = productOptions[0].ImageUrl;
            return product;
        }

        public async Task<ListProductForAdmin> productViewForAdmins(int pageNumber, int pageSize, int? categoryId,
            int? productCateId, string? key)
        {
            ListProductForAdmin listProductForAdmin = new ListProductForAdmin();

            var query = from p in _context.Products
                        join pc in _context.ProductCategories on p.ProductCateId equals pc.ProductCateId
                        join c in _context.Categories on pc.CategoryId equals c.CategoryId
                        select new
                        {
                            id = p.ProductId,
                            productCategory = pc,
                            category = c,
                            isDelete = p.IsDelete,
                            name = p.Name
                        };

            // Apply filters
            if (productCateId.HasValue)
            {
                query = query.Where(p => p.productCategory.ProductCateId == productCateId.Value);
            }
            else if (categoryId.HasValue)
            {
                query = query.Where(p => p.category.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(p => p.name.Contains(key));
            }

            // Fetch initial filtered list
            var initialResult = await query.ToListAsync();

            // Project to ProductViewForAdmin and fetch related data in memory
            var products = initialResult.Select(p => new ProductViewForAdmin
            {
                Id = p.id,
                Name = p.name,
                ProductCategory = new ProductCategoryViewModel
                {
                    Id = p.productCategory.ProductCateId,
                    Name = p.productCategory.Name
                },
                Category = new CategoryViewModel
                {
                    Id = p.category.CategoryId,
                    Name = p.category.Name
                },
                // Initialize other fields to default values
                Price = 0,
                IsSoldOut = false,
                ImgUrl = string.Empty,
                SoldQuantity = 0,
                IsDelete = p.isDelete
            }).ToList();

            // Total products count
            listProductForAdmin.totalProducts = products.Count;

            products = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var productIds = products.Select(p => p.Id).ToList();

            var productOptions = await (from po in _context.ProductOptions
                                        join i in _context.Images on po.ImageId equals i.ImageId
                                        where productIds.Contains(po.ProductId)
                                        select new
                                        {
                                            po.ProductId,
                                            po.Price,
                                            po.IsSoldOut,
                                            ImageUrl = i.ImageUrl
                                        }).ToListAsync();

            var soldQuantities = await (from po in _context.ProductOptions
                                        join or in _context.OrderItems on po.ProductOptionId equals or.ProductOptionId
                                        where productIds.Contains(po.ProductId)
                                        group or by po.ProductId into g
                                        select new
                                        {
                                            ProductId = g.Key,
                                            SoldQuantity = g.Sum(x => x.Quantity)
                                        }).ToListAsync();

            foreach (var product in products)
            {
                var options = productOptions.Where(po => po.ProductId == product.Id).ToList();
                product.IsSoldOut = !options.Exists(po => !po.IsSoldOut);
                var firstOption = options.FirstOrDefault();
                if (firstOption != null)
                {
                    product.ImgUrl = firstOption.ImageUrl;
                    product.Price = firstOption.Price;
                }

                var soldQuantity = soldQuantities.Find(sq => sq.ProductId == product.Id)?.SoldQuantity ?? 0;
                product.SoldQuantity = soldQuantity;
            }

            // Apply pagination
            listProductForAdmin.products = products;

            return listProductForAdmin;
        }


        public int GetTotalProducts(List<ProductViewForAdmin> products)
        {
            var count = products.Count;
            return count;
        }

        public async Task<string> CreateProduct(ProductCreateRequestViewModel productCreateRequest)
        {
            try
            {
                var productId = _context.Products.Max(p => p.ProductId) + 1;

                var brandId = productCreateRequest.Brand.BrandId;

                if (brandId == 0)
                {
                    var brand = new Models.Brand
                    {
                        Name = productCreateRequest.Brand.Name,
                        IsDelete = false
                    };

                    _context.Brands.Add(brand);
                    _context.SaveChanges();

                    brandId = brand.BrandId;
                }

                var productCateId = productCreateRequest.ProductCategory.ProductCateId;
                if (productCateId == 0)
                {
                    productCateId = _context.ProductCategories.Select(pc => pc.ProductCateId).Max() + 1;
                    var productCate = new Models.ProductCategory
                    {
                        ProductCateId = productCateId,
                        Name = productCreateRequest.ProductCategory.Name,
                        CategoryId = productCreateRequest.CategoryId
                    };
                    _context.ProductCategories.Add(productCate);
                    _context.SaveChanges();
                }

                var product = new Models.Product
                {
                    ProductId = productId,
                    Name = productCreateRequest.Name,
                    Description = productCreateRequest.Description,
                    BrandId = brandId,
                    ProductCateId = productCateId,
                    IsDelete = false
                };

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                HashSet<string> string_size = new HashSet<string>();
                HashSet<string> string_image = new HashSet<string>();
                HashSet<string> string_attribute = new HashSet<string>();

                List<Models.Image> images = new List<Models.Image>();
                List<Models.Size> sizes = new List<Models.Size>();
                List<Models.Attribute> attributes = new List<Models.Attribute>();

                foreach (var option in productCreateRequest.ProductOptions)
                {
                    var sizeId = option.Size.SizeId;

                    if (string_size.Add(option.Size.Name))
                    {
                        if (sizeId == 0)
                        {
                            sizeId = _size.CreateSize(option.Size.Name);
                            option.Size.SizeId = sizeId;
                        }
                        sizes.Add(option.Size);
                    }

                    else
                    {
                        sizeId = sizes.Find(s => s.Name == option.Size.Name).SizeId;
                    }

                    var attributeId = option.Attribute.AttributeId;

                    if (string_attribute.Add(option.Attribute.Name))
                    {
                        if (attributeId == 0)
                        {
                            attributeId = _attribute.CreateAttribute(option.Attribute.Name);
                            option.Attribute.AttributeId = attributeId;
                        }
                        attributes.Add(option.Attribute);
                    }
                    else
                    {
                        attributeId = attributes.Find(a => a.Name == option.Attribute.Name).AttributeId;
                    }

                    var imageId = option.Image.ImageId;
                    if (string_image.Add(option.Image.ImageUrl))
                    {
                        if (imageId == 0)
                        {
                            var result = await _image.CreateImage(option.Image.ImageUrl);
                            if (!int.TryParse(result, out int number))
                            {
                                return result;
                            }
                            imageId = int.Parse(result);
                            option.Image.ImageId = imageId;
                        }
                        images.Add(option.Image);
                    }
                    else
                    {
                        imageId = images.Find(i => i.ImageUrl == option.Image.ImageUrl).ImageId;
                    }

                    var productOption = new Models.ProductOption
                    {
                        ProductId = product.ProductId,
                        SizeId = sizeId,
                        AttributeId = attributeId,
                        Price = option.Price,
                        ImageId = imageId,
                        IsSoldOut = false,
                        IsDelete = false
                    };

                    await _context.ProductOptions.AddAsync(productOption);
                }
                await _context.SaveChangesAsync();
                return productId.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public ProductDetailForAdmin GetProductDetailForAdmin(int productId)
        {
            var productOptions = (from po in _context.ProductOptions
                                  join a in _context.Attributes on po.AttributeId equals a.AttributeId
                                  join s in _context.Sizes on po.SizeId equals s.SizeId
                                  join i in _context.Images on po.ImageId equals i.ImageId
                                  where po.ProductId == productId
                                  select new ProductOptionDetailForAdmin
                                  {
                                      Id = po.ProductOptionId,
                                      Size = new Models.Size()
                                      {
                                          SizeId = s.SizeId,
                                          Name = s.Name
                                      },
                                      Price = po.Price,
                                      Image = new Models.Image()
                                      {
                                          ImageId = i.ImageId,
                                          ImageUrl = i.ImageUrl
                                      },
                                      Attribute = new Models.Attribute()
                                      {
                                          AttributeId = a.AttributeId,
                                          Name = a.Name
                                      },
                                      IsSoldOut = po.IsSoldOut,
                                      IsDelete = po.IsDelete
                                  }).ToList();

            foreach (var item in productOptions)
            {
                item.SoldQuantity = _context.OrderItems.Where(x => x.ProductOptionId == item.Id).Select(x => x.Quantity).Sum();
            }

            var product = (from p in _context.Products
                           join b in _context.Brands on p.BrandId equals b.BrandId
                           join pc in _context.ProductCategories on p.ProductCateId equals pc.ProductCateId
                           join c in _context.Categories on pc.CategoryId equals c.CategoryId
                           where p.ProductId == productId
                           select new ProductDetailForAdmin
                           {
                               ProductId = p.ProductId,
                               Name = p.Name,
                               Brand = new Models.Brand()
                               {
                                   BrandId = b.BrandId,
                                   Name = b.Name
                               },
                               Description = p.Description,
                               IsDeleted = p.IsDelete,
                               Category = new Models.Category()
                               {
                                   CategoryId = c.CategoryId,
                                   Name = c.Name
                               },
                               ProductCategory = new Models.ProductCategory()
                               {
                                   ProductCateId = pc.ProductCateId,
                                   Name = pc.Name
                               },
                               ProductOptions = productOptions
                           }).FirstOrDefault();
            foreach (var item in productOptions)
            {
                item.Image.ImageUrl = formatUrl(item.Image.ImageUrl);
            }

            if (product != null)
            {
                product.IsSoldOut = !productOptions.Any(p => p.IsSoldOut == false);
                if (product.IsDeleted == true)
                {
                    foreach (var item in product.ProductOptions)
                    {
                        item.IsDelete = true;
                    }
                }
            }

            return product;
        }

        public async Task<string> UpdateProduct(ProductDetailForAdmin productUpdateRequest)
        {
            var isDelete = productUpdateRequest.IsDeleted;

            var brandId = productUpdateRequest.Brand.BrandId;
            if (brandId == 0)
            {
                var brand = new Models.Brand
                {
                    Name = productUpdateRequest.Brand.Name
                };

                _context.Brands.Add(brand);
                _context.SaveChanges();

                brandId = brand.BrandId;
            }

            var productCateId = productUpdateRequest.ProductCategory.ProductCateId;
            if (productCateId == 0)
            {
                productCateId = _context.ProductCategories.Select(pc => pc.ProductCateId).Max() + 1;
                var productCate = new Models.ProductCategory
                {
                    ProductCateId = productCateId,
                    Name = productUpdateRequest.ProductCategory.Name,
                    CategoryId = productUpdateRequest.Category.CategoryId
                };
                _context.ProductCategories.Add(productCate);
                _context.SaveChanges();
            }

            var product = await _context.FindAsync<Models.Product>(productUpdateRequest.ProductId);

            if (product != null)
            {
                product.Name = productUpdateRequest.Name;
                product.BrandId = brandId;
                product.Description = productUpdateRequest.Description;
                product.ProductCateId = productCateId;
                product.IsDelete = productUpdateRequest.IsDeleted;
                _context.Products.Update(product);
            }

            HashSet<string> string_size = new HashSet<string>();
            HashSet<string> string_image = new HashSet<string>();
            HashSet<string> string_attribute = new HashSet<string>();

            List<Models.Image> images = new List<Models.Image>();

            List<Models.Size> sizes = new List<Models.Size>();
            List<Models.Attribute> attributes = new List<Models.Attribute>();

            var options = productUpdateRequest.ProductOptions;
            foreach (var option in options)
            {
                var sizeId = option.Size.SizeId;

                if (string_size.Add(option.Size.Name))
                {
                    if (sizeId == 0)
                    {
                        sizeId = _size.CreateSize(option.Size.Name);
                        option.Size.SizeId = sizeId;
                    }
                    sizes.Add(option.Size);
                }
                else
                {
                    sizeId = sizes.Find(s => s.Name == option.Size.Name).SizeId;
                }

                var attributeId = option.Attribute.AttributeId;

                if (string_attribute.Add(option.Attribute.Name))
                {
                    if (attributeId == 0)
                    {
                        attributeId = _attribute.CreateAttribute(option.Attribute.Name);
                        option.Attribute.AttributeId = attributeId;
                    }
                    attributes.Add(option.Attribute);
                }
                else
                {
                    attributeId = attributes.Find(a => a.Name == option.Attribute.Name).AttributeId;
                }

                var imageId = option.Image.ImageId;
                if (string_image.Add(option.Image.ImageUrl))
                {
                    if (imageId == 0)
                    {
                        var result = await _image.CreateImage(option.Image.ImageUrl);
                        if (!int.TryParse(result, out int number))
                        {
                            return result;
                        }
                        option.Image.ImageId = int.Parse(result);
                        imageId = option.Image.ImageId;
                    }
                    images.Add(option.Image);
                }
                else
                {
                    imageId = images.Find(i => i.ImageUrl == option.Image.ImageUrl).ImageId;
                }


                var productOptionId = option.Id;

                if (productOptionId == 0)
                {
                    var productOption = new Models.ProductOption
                    {
                        ProductId = productUpdateRequest.ProductId,
                        SizeId = sizeId,
                        AttributeId = attributeId,
                        Price = option.Price,
                        ImageId = imageId,
                        IsSoldOut = option.IsSoldOut,
                        IsDelete = option.IsDelete
                    };

                    await _context.ProductOptions.AddAsync(productOption);
                }
                else
                {
                    var x = isDelete == true ? isDelete : option.IsDelete;
                    var productOption = await _context.FindAsync<Models.ProductOption>(productOptionId);
                    if (productOption != null)
                    {
                        productOption.AttributeId = attributeId;
                        productOption.SizeId = sizeId;
                        productOption.ImageId = imageId;
                        productOption.IsDelete = x;
                        productOption.IsSoldOut = option.IsSoldOut;
                        productOption.Price = option.Price;
                        _context.Update(productOption);
                    }

                }

            }
            await _context.SaveChangesAsync();
            return productUpdateRequest.ProductId.ToString();

        }

        public void DeleteProduct(int productId)
        {
            var product = _context.Products.Find(productId);
            product.IsDelete = true;
            _context.Products.Update(product);
            _context.SaveChangesAsync();
        }

    }
}

