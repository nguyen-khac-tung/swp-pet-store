﻿using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Product;

namespace PetStoreProject.Controllers
{
	[Route("[controller]/[action]")]
	public class ProductController : Controller
	{
		private readonly IProductRepository _product;

		public ProductController(IProductRepository product)
		{
			_product = product;
		}

		[HttpGet("{productId}")]
		public ActionResult Detail(int productId)
		{
			var product_detail = _product.GetDetail(productId);
			ViewData["product_detail"] = product_detail;
			ViewData["related_products"] = _product.getRelatedProduct(productId);
			return View();
		}

		[HttpPost]
		public ActionResult quickPreview(int productId)
		{
			var product_detail = _product.GetDetail(productId);
			return Json(product_detail);
		}


	}
}
