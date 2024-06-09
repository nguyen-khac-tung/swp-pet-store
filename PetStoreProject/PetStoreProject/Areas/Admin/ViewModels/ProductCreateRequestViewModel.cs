using System.ComponentModel.DataAnnotations;
namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductCreateRequestViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm tối đa 200 kí tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả sản phẩm")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Cần lòng cho biết sản phẩm thuộc danh mục nào")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Cần cho biết sản phẩm thuộc những loại danh mục sản phẩm nào")]
        public int ProductCateId { get; set; }

        [Required(ErrorMessage = "Cần cung cấp thương hiệu của sản phẩm")]
        public int BrandId { get; set; }

        [Required]
        public List<ProductOptionCreateRequestViewModel> Options { get; set; }
    }
}
