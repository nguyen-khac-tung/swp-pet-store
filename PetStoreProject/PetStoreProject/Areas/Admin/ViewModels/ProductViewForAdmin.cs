﻿namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductViewForAdmin
    {
        public int id { get; set; }
        public string name { get; set; }
        public string imgUrl { get; set; }
        public float price { get; set; }
        public bool isSoldOut { get; set; }
        public int soldQuantity { get; set; }
    }
}
