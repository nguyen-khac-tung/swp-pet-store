using PetStoreProject.Models;

namespace PetStoreProject.Areas.Employee.ViewModels
{
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }

        public int? CustomerId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string ShipAddress { get; set; }

        public string PaymentMethod { get; set; }

        public double TotalAmount { get; set; }

        public DateTime OrderDate { get; set; }

        public int totalOrderItems { get; set; } = 0;
    }
}
