using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductOptionCreateRequestViewModel
    {
        [Required(ErrorMessage = "Vui lòng cung cấp ảnh sản phẩm")]
        public string ImageData { get; set; }

        public int? SizeId { get; set; }
        public int? AttributeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        //[DecimalPrecision(3, ErrorMessage = "Giá sản phẩm chỉ được phép có tối đa 3 chữ số sau dấu thập phân.")]
        public float Price { get; set; }
    }
}
