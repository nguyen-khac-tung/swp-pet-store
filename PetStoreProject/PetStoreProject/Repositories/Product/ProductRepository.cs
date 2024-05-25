﻿using PetStoreProject.Models;
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
                                      }
                                  }).ToList();

            var attributes = productOptions.Select(a => a.attribute)
                               .GroupBy(a => a.AttributeId) // GroupBy theo ID hoặc thuộc tính duy nhất
                               .Select(g => g.First()) // Chọn phần tử đầu tiên từ mỗi nhóm
                               .ToList();

            var sizes = productOptions.Select(s => s.size)
                               .GroupBy(s => s.SizeId) // GroupBy theo ID hoặc thuộc tính duy nhất
                               .Select(g => g.First()) // Chọn phần tử đầu tiên từ mỗi nhóm
                               .ToList();


            foreach (var image in images)
            {
                image.ImageUrl = formatUrl(image.ImageUrl);
            }
            product.attributes = attributes;
            product.sizes = sizes;
            product.images = images;
            product.productOption = productOptions;

            return product;
        }

        public List<RelatedProductViewModel> getRelatedProduct(int productId)
        {
            var cateId = (from p in _context.Products where p.ProductId == productId select p.ProductCateId).FirstOrDefault();
            var products = (from po in _context.ProductOptions
                            join p in _context.Products on po.ProductId equals p.ProductId
                            join b in _context.Brands on p.BrandId equals b.BrandId
                            join i in _context.Images on po.ImageId equals i.ImageId
                            where p.ProductCateId == cateId
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

        public List<Models.Product> GetProductsByCategories(List<int> categoriesIds)

        {
            List<Models.Product> products = (from p in _context.Products
                                             join pc in _context.ProductCategories on p.ProductCateId equals pc.ProductCateId
                                             where categoriesIds.Contains(pc.CategoryId)
                                             select p).ToList();
            return products;
        }

        public List<Models.Product> GetAllAccessories()
        {
            List<int> categoryIds = [2, 5, 6];
            return GetProductsByCategories(categoryIds);
        }

        public List<Models.Product> GetAllFoods()
        {
            List<int> categoryIds = [1, 3, 4];
            return GetProductsByCategories(categoryIds);
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
                                      img_url = i.ImageUrl

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

        public List<Brand> GetBrandByCategoryId(List<int> categoryIds)
        {

            var brands = (from b in _context.Brands
                          join p in _context.Products on b.BrandId equals p.BrandId
                          join pc in _context.ProductCategories on p.ProductCateId equals pc.ProductCateId
                          where categoryIds.Contains(pc.CategoryId)
                          select b).Distinct().ToList();
            return brands;
        }
        public List<Brand> GetBrandAccessories()
        {
            List<int> categoryIds = [2, 5, 6];
            return GetBrandByCategoryId(categoryIds);
        }

        public List<ProductDetailViewModel> GetProductDetailAccessories()
        {

            var products = GetAllAccessories();
            var productDetails = products.Select(p => new ProductDetailViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Brand = GetBrandByProductId(p.ProductId).Name,
                //Description = p.Description,
                productOption = GetProductOptionsByProductId(p.ProductId),
                images = GetImagesByProductId(p.ProductId),
                //attributes = GetAttributesByProductId(p.ProductId),
                //sizes = GetSizesByProductId(p.ProductId)


            }).ToList();
            return productDetails;
        }

    }

    internal record NewRecord(int ProductId, string Name, string Item);
}
