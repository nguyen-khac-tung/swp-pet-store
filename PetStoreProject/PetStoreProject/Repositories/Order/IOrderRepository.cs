﻿using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Order
{
    public interface IOrderRepository
    {
        public List<OrderDetailViewModel> GetOrderDetailByCondition(OrderModel orderModel);

        public int GetCountOrder(OrderModel orderCondition);

        public void AddOrder(Models.Order order);

        public float GetTotalProductSale();

        public OrderDetailViewModel? GetOrderDetailById(long orderId);

        public void UpdateStatusOrder(long orderId, string status, int shipper);

        public void UpdateReturnOrder(long orderId, int returnId);
    }
}
