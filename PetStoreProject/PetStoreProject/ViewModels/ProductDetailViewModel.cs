﻿using PetStoreProject.Models;
using Attribute = PetStoreProject.Models.Attribute;

namespace PetStoreProject.ViewModels
{
	public class ProductDetailViewModel
	{
		public int ProductId { get; set; }
		public string Name { get; set; }
		public string Brand { get; set; }
		public string Description { get; set; }
		public List<ProductOptionViewModel> productOption { get; set; }
		public List<Image> images { get; set; }
		public List<Attribute>? attributes { get; set; }
		public List<Size>? sizes { get; set; }
    }
}
