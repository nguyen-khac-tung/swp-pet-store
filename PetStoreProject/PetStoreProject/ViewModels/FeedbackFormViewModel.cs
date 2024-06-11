using PetStoreProject.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.ViewModels
{
    public class FeedbackFormViewModel
    {
        [Required(ErrorMessage = "Tên không được bỏ trống")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại của bạn")]
        [RegularExpression("^\\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập lại.")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(150, ErrorMessage = "Tối đa 150 ký tự")]
        public string Email { get; set; }

        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự")]
        public string Content { get; set; }
    }
}
