﻿using Attribute = PetStoreProject.Models.Attribute;
using Size = PetStoreProject.Models.Size;
namespace PetStoreProject.ViewModels
{
    public class CartItemVM
    {
        public int ProductId { get; set; }
        public int ProductOptionId { get; set; }
        public string Name { get; set; }
        public Attribute? Attribute { get; set; }
        public Size? Size { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public string ImgUrl { get; set; }
    }
}