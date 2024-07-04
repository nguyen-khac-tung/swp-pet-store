﻿using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Discount
{
    public interface IDiscountRepository
    {
        public string Create(Models.Discount discount);
        public List<DiscountViewModel> GetDiscounts();
    }
}
