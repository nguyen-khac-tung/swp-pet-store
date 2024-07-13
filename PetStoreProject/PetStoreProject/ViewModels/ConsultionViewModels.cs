using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PetStoreProject.ViewModels
{
    public class ConsultionViewModels
    {
        public int ConsultionId { get; set; }
        public string CustomerName { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? EmployeeId { get; set; }
        public bool? Status { get; set; }
    }
}
