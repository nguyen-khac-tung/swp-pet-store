using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [DisplayName("Họ và Tên")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        public string DoB { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression("^\\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập lại.")]
        [DisplayName("Số Điện Thoại")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [DisplayName("Địa Chỉ")]
        [StringLength(100, ErrorMessage = "Tối đa 150 ký tự")]
        public string Address { get; set; }

        //[Required(ErrorMessage = "Vui lòng nhập mật khẩu của bạn")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Vui lòng nhập mật khẩu có độ dài từ 8 đến 20 ký tự.")]
        [DisplayName("Mật Khẩu")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }


    }
}
